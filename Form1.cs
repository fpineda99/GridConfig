using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;
using System.Diagnostics.PerformanceData;
using T3D = Tekla.Structures.Geometry3d;

namespace GridConfig
{
    public partial class Form1 : Form
    {
        private Model model;
        public Form1()
        {
            InitializeComponent();

            model = new Model();

            // load existing grid data when the form starts
            LoadExistingGridData();
        }

        private void btnCreateGrid_Click(object sender, EventArgs e)
        {
            try
            {
                if (!model.GetConnectionStatus())
                {
                    MessageBox.Show("Cannot connect to Tekla", "Connection Error");
                    return;
                }

                // Get the coordinates from the text boxes
                string xAxisInput = txtX.Text;
                string yAxisInput = txtY.Text;
                string zAxisInput = txtZ.Text;

                // Parse the coordinates
                Dictionary<string, double> xCoordinates = ParseCoordinates(xAxisInput);
                Dictionary<string, double> yCoordinates = ParseCoordinates(yAxisInput);
                Dictionary<string, double> zCoordinates = ParseCoordinates(zAxisInput);

                // Decompose any combined labels (like "ABC") into individual labels for X and Y only
                Dictionary<string, double> decomposedXCoordinates = DecomposeDimensions(xCoordinates);
                Dictionary<string, double> decomposedYCoordinates = DecomposeDimensions(yCoordinates);

                // Create the grid
                CreateGrid(decomposedXCoordinates, decomposedYCoordinates, zCoordinates);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating grid: {ex.Message}", "Error");
            }
        }

        private Dictionary<string, double> ParseCoordinates(string input)
        {
            Dictionary<string, double> coordinates = new Dictionary<string, double>();

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Warning: Empty coordinate input detected. Please provide coordinates in the format 'Label:Value' (e.g., 'A:0 B:1500 C:3000').", "Input Warning");
                return coordinates;
            }

            // Turn a string like "A:0 B:1500 C:3000" into pairs like "A:0" "B:1500" "C:3000"
            foreach (string pair in input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                // Split the pair into key and value e.g. "A:0" -> ["A", "1000"]
                string[] parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    if (double.TryParse(parts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                        coordinates[key] = value;
                    else
                        MessageBox.Show($"Invalid dimension value: {parts[1].Trim()} for key: {key}");
                }
                else
                {
                    MessageBox.Show($"Invalid dimension format: {pair}");
                }
            }

            return coordinates;
        }
        private void cumulateDimensions(Dictionary<string, double> dimensions)
        {
            double totalValue = 0;
            List<string> keys = new List<string>(dimensions.Keys);

            foreach (var key in keys)
            {
                totalValue += dimensions[key];
                totalValue = Math.Round(totalValue, 6);
                dimensions[key] = totalValue;
            }
        }

        private void deleteExistingGrid()
        {
            try
            {
                // Step 1: Get all grid objects and delete them
                ModelObjectEnumerator existingGrids = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.GRID);
                List<ModelObject> objectsToDelete = new List<ModelObject>();

                while (existingGrids.MoveNext())
                    objectsToDelete.Add(existingGrids.Current);

                // Delete each grid
                foreach (ModelObject obj in objectsToDelete)
                    obj.Delete();

                // Step 2: Delete any grid planes
                objectsToDelete.Clear();
                ModelObjectEnumerator existingGridPlanes = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.GRIDPLANE);

                while (existingGridPlanes.MoveNext())
                    objectsToDelete.Add(existingGridPlanes.Current);

                foreach (ModelObject obj in objectsToDelete)
                    obj.Delete();

                // Final commit to ensure everything is deleted before creating a new grid
                model.CommitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Warning: Failed to delete existing grids: {ex.Message}", "Warning");
            }
        }
        private void CreateGrid(Dictionary<string, double> xCoords, Dictionary<string, double> yCoords, Dictionary<string, double> zCoords)
        {
            try
            {
                // Get current transformation plane
                TransformationPlane currentPlane = model.GetWorkPlaneHandler().GetCurrentTransformationPlane();

                // First make sure no grids exist
                deleteExistingGrid();

                // Process cumulative dimensions
                cumulateDimensions(xCoords);
                cumulateDimensions(yCoords);
                cumulateDimensions(zCoords);

                // Get the maximum extents for the grid planes
                double maxX = xCoords.Count > 0 ? xCoords.Values.Max() : 10000;
                double maxY = yCoords.Count > 0 ? yCoords.Values.Max() : 10000;
                double maxZ = zCoords.Count > 0 ? zCoords.Values.Max() : 10000;

                // Use fixed extension buffer
                const double extensionBuffer = 100;
                double gridExtentX = maxX + extensionBuffer;
                double gridExtentY = maxY + extensionBuffer;
                double gridExtentZ = maxZ + extensionBuffer;

                // Create a master Grid object - this is needed as the parent for GridPlanes
                Grid masterGrid = new Grid();
                masterGrid.Name = "UserDefinedGrid";

                // Insert the master grid
                if (!masterGrid.Insert())
                    throw new Exception("Failed to insert the master grid");

                // Commit the grid creation before adding planes
                model.CommitChanges();

                // Create the X grid planes (vertical planes along X axis)
                foreach (var xCoord in xCoords)
                {
                    try
                    {
                        // Create a new GridPlane
                        GridPlane gridPlane = new GridPlane();

                        Plane plane = new Plane();
                        // The X planes have an origin that uses the X coordinate
                        plane.Origin = new T3D.Point(xCoord.Value, -extensionBuffer, -extensionBuffer);
                        plane.AxisX = new Vector(0, gridExtentY + extensionBuffer, 0);
                        plane.AxisY = new Vector(0, 0, gridExtentZ + extensionBuffer);

                        // Set properties
                        gridPlane.Label = xCoord.Key;
                        gridPlane.Plane = plane;
                        gridPlane.IsMagnetic = true;
                        gridPlane.Parent = masterGrid;

                        // Insert the grid plane
                        bool inserted = gridPlane.Insert();
                        if (!inserted)
                            MessageBox.Show($"Failed to insert X plane {xCoord.Key}", "Warning");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating X plane {xCoord.Key}: {ex.Message}", "Error");
                    }
                }

                // Create the Y grid planes (vertical planes along Y axis)
                foreach (var yCoord in yCoords)
                {
                    try
                    {
                        // Create a new GridPlane
                        GridPlane gridPlane = new GridPlane();

                        // Create a plane with much larger extent to ensure intersections
                        Plane plane = new Plane();
                        // The Y planes have an origin that uses the Y coordinate
                        plane.Origin = new T3D.Point(-extensionBuffer, yCoord.Value, -extensionBuffer);
                        plane.AxisX = new Vector(gridExtentX + extensionBuffer, 0, 0);
                        plane.AxisY = new Vector(0, 0, gridExtentZ + extensionBuffer);

                        // Set properties
                        gridPlane.Label = yCoord.Key;
                        gridPlane.Plane = plane;
                        gridPlane.IsMagnetic = true;
                        gridPlane.Parent = masterGrid;

                        // Insert the grid plane
                        bool inserted = gridPlane.Insert();
                        if (!inserted)
                            MessageBox.Show($"Failed to insert Y plane {yCoord.Key}", "Warning");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating Y plane {yCoord.Key}: {ex.Message}", "Error");
                    }
                }

                // Create the Z grid planes (horizontal planes)
                foreach (var zCoord in zCoords)
                {
                    try
                    {
                        // Create a new GridPlane
                        GridPlane gridPlane = new GridPlane();

                        // Create a plane with much larger extent to ensure intersections
                        Plane plane = new Plane();
                        plane.Origin = new T3D.Point(-extensionBuffer, -extensionBuffer, zCoord.Value);
                        plane.AxisX = new Vector(gridExtentX + extensionBuffer, 0, 0);
                        plane.AxisY = new Vector(0, gridExtentY + extensionBuffer, 0);

                        // Set properties
                        gridPlane.Label = zCoord.Key;
                        gridPlane.Plane = plane;
                        gridPlane.IsMagnetic = true;
                        gridPlane.Parent = masterGrid;

                        // Insert the grid plane
                        bool inserted = gridPlane.Insert();
                        if (!inserted)
                            MessageBox.Show($"Failed to insert Z plane {zCoord.Key}", "Warning");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating Z plane {zCoord.Key}: {ex.Message}", "Error");
                    }
                }

                model.CommitChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating grid: {ex.Message}");
            }
        }

        private Dictionary<string, double> CombineDimensionsWithSameValues(Dictionary<string, double> dimensions)
        {
            // Create a result dictionary
            var result = new Dictionary<string, double>();

            // Sort dimensions by value to ensure planes with the same coordinates are grouped together
            var orderedDimensions = dimensions.OrderBy(kv => kv.Value).ToList();

            // Process dimensions
            int i = 0;
            while (i < orderedDimensions.Count)
            {
                string currentKey = orderedDimensions[i].Key;
                double currentValue = orderedDimensions[i].Value;

                // Find consecutive dimensions with the same value
                List<string> groupKeys = new List<string> { currentKey };
                int j = i + 1;

                while (j < orderedDimensions.Count &&
                      Math.Abs(orderedDimensions[j].Value - currentValue) < 0.001)
                {
                    groupKeys.Add(orderedDimensions[j].Key);
                    j++;
                }

                // Combine consecutive keys with the same value
                if (groupKeys.Count > 1)
                {
                    // Sort the keys alphabetically for consistent combined keys
                    groupKeys.Sort();
                    string combinedKey = string.Join("", groupKeys);
                    result[combinedKey] = currentValue;
                }
                else
                {
                    result[currentKey] = currentValue;
                }

                // Move to the next unprocessed dimension
                i = j;
            }

            return result;
        }

        private Dictionary<string, double> DecomposeDimensions(Dictionary<string, double> dimensions)
        {
            var result = new Dictionary<string, double>();

            foreach (var dimension in dimensions)
            {
                string combinedKey = dimension.Key;
                double value = dimension.Value;

                // Simple case: key doesn't need decomposition
                if (combinedKey.Length == 1)
                {
                    result[combinedKey] = value;
                    continue;
                }

                // Process complex key: we need to decompose it
                int i = 0;
                while (i < combinedKey.Length)
                {
                    // Start of a single key
                    string singleKey = combinedKey[i].ToString();
                    i++;

                    // Check if there's a period after the current character
                    if (i < combinedKey.Length && combinedKey[i] == '.')
                    {
                        // Add the period to the current key
                        singleKey += '.';
                        i++;

                        // Add all consecutive digits after the period
                        while (i < combinedKey.Length && char.IsDigit(combinedKey[i]))
                        {
                            singleKey += combinedKey[i];
                            i++;
                        }
                    }

                    // Add the decomposed dimension
                    result[singleKey] = value;
                }
            }

            return result;
        }

        private void LoadExistingGridData()
        {
            try
            {
                // Check if connected to Tekla
                if (!model.GetConnectionStatus())
                    return;

                // Collections to store the coordinates for each axis
                Dictionary<string, double> xGrids = new Dictionary<string, double>();
                Dictionary<string, double> yGrids = new Dictionary<string, double>();
                Dictionary<string, double> zGrids = new Dictionary<string, double>();

                // Get all grid planes from the model
                ModelObjectEnumerator gridPlanes = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.GRIDPLANE);

                // Process each grid plane
                while (gridPlanes.MoveNext())
                {
                    GridPlane gridPlane = gridPlanes.Current as GridPlane;
                    if (gridPlane == null || string.IsNullOrEmpty(gridPlane.Label))
                        continue;

                    Plane plane = gridPlane.Plane;
                    string label = gridPlane.Label;

                    // Determine axis by plane orientation and label format
                    DetermineGridAxis(plane, label, xGrids, yGrids, zGrids);
                }

                // Convert to absolute coordinates to relative spacings for X and Y axes only
                Dictionary<string, double> relativeXGrids = ConvertToRelativeSpacings(xGrids);
                Dictionary<string, double> relativeYGrids = ConvertToRelativeSpacings(yGrids);
                Dictionary<string, double> relativeZGrids = ConvertToRelativeSpacings(zGrids);

                // Combine dimensions with same values for X and Y axes only
                Dictionary<string, double> combinedXGrids = CombineDimensionsWithSameValues(relativeXGrids);
                Dictionary<string, double> combinedYGrids = CombineDimensionsWithSameValues(relativeYGrids);

                // Format output for each axis
                if (xGrids.Count > 0) txtX.Text = FormatGridsForDisplay(combinedXGrids);
                if (yGrids.Count > 0) txtY.Text = FormatGridsForDisplay(combinedYGrids);
                if (zGrids.Count > 0) txtZ.Text = FormatGridsForDisplay(relativeZGrids);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading grid data: {ex.Message}", "Error");
            }
        }

        private Dictionary<string, double> ConvertToRelativeSpacings(Dictionary<string, double> absoluteGrids)
        {
            var result = new Dictionary<string, double>();

            // Sort by absolute position
            var sortedGrids = absoluteGrids.OrderBy(kv => kv.Value).ToList();

            // Convert to relative spacings
            double previousPosition = 0;
            for (int i = 0; i < sortedGrids.Count; i++)
            {
                string label = sortedGrids[i].Key;
                double absolutePosition = sortedGrids[i].Value;
                double relativeSpacing = (i == 0) ? absolutePosition : absolutePosition - previousPosition;

                // Round to minimize floating point issues
                relativeSpacing = Math.Round(relativeSpacing, 2);

                result[label] = relativeSpacing;
                previousPosition = absolutePosition;
            }

            return result;
        }

        private string FormatGridsForDisplay(Dictionary<string, double> gridSpacings)
        {
            // Just convert the dictionary to a formatted string without changing the values
            StringBuilder result = new StringBuilder();

            // Sort by key for consistent display
            var sortedGrids = gridSpacings.OrderBy(kv => kv.Key).ToList();

            for (int i = 0; i < sortedGrids.Count; i++)
            {
                if (i > 0) result.Append(" ");
                result.Append($"{sortedGrids[i].Key}:{sortedGrids[i].Value.ToString(CultureInfo.InvariantCulture)}");
            }

            return result.ToString();
        }

        private void DetermineGridAxis(Plane plane, string label,
            Dictionary<string, double> xGrids, Dictionary<string, double> yGrids, Dictionary<string, double> zGrids)
        {
            // Get dot products to check alignment with global axes
            Vector globalX = new Vector(1, 0, 0);
            Vector globalY = new Vector(0, 1, 0);
            Vector globalZ = new Vector(0, 0, 1);

            // Calculate dot products to determine plane orientation
            double dotX1 = DotProduct(plane.AxisX, globalX);
            double dotY2 = DotProduct(plane.AxisY, globalY);

            const double threshold = 0.7; // Threshold for alignment

            // Check if this is a horizontal plane (Z grid)
            if (Math.Abs(dotX1) > threshold && Math.Abs(dotY2) > threshold)
            {
                zGrids[label] = plane.Origin.Z;
                return;
            }

            // Check label format
            bool isNumericLabel = char.IsDigit(label[0]);

            // Calculate normal vector to help determine plane orientation
            Vector normal = GetPlaneNormalVector(plane);

            // Vertical planes - check alignment with global axes
            if (Math.Abs(normal.X) > threshold)
            {
                xGrids[label] = plane.Origin.X;
                return;
            }
            else if (Math.Abs(normal.Y) > threshold)
            {
                yGrids[label] = plane.Origin.Y;
                return;
            }

            // Fallback for cases that don't align well with axes
            // Determine based on label format
            if (Math.Abs(normal.Z) > threshold)
                zGrids[label] = plane.Origin.Z;
            else if (isNumericLabel)
                yGrids[label] = plane.Origin.Y;  // Numeric labels go to Y-axis
            else
                xGrids[label] = plane.Origin.X;  // Alphabetic labels go to X-axis
        }

        private double DotProduct(Vector v1, Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        private Vector GetPlaneNormalVector(Plane plane)
        {
            // Calculate the normal vector of the plane using cross product
            Vector normal = new Vector();
            normal.X = plane.AxisX.Y * plane.AxisY.Z - plane.AxisX.Z * plane.AxisY.Y;
            normal.Y = plane.AxisX.Z * plane.AxisY.X - plane.AxisX.X * plane.AxisY.Z;
            normal.Z = plane.AxisX.X * plane.AxisY.Y - plane.AxisX.Y * plane.AxisY.X;

            // Normalize the vector
            double length = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
            if (length > 0)
            {
                normal.X /= length;
                normal.Y /= length;
                normal.Z /= length;
            }

            return normal;
        }
    }
}