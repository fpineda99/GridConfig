using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

using Tekla.Structures;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;
using T3D = Tekla.Structures.Geometry3d;

namespace GridConfigV2
{
    public partial class Form1 : Form
    {
        private Model model;
        
        public Form1()
        {
            model = new Model();
            InitializeComponent();
            fillTextBoxes();
        }

        public string CompressNumberSequence(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // First expand any multiplier notation like "4*3000.00"
            List<double> numbers = new List<double>();
            string[] parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string part in parts)
            {
                if (part.Contains("*"))
                {
                    // Handle multiplier notation (e.g., "4*3000.00")
                    string[] multParts = part.Split('*');
                    if (multParts.Length == 2 && int.TryParse(multParts[0], out int multCount) && 
                        double.TryParse(multParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                    {
                        for (int idx = 0; idx < multCount; idx++)
                        {
                            numbers.Add(value);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Could not parse multiplier notation '{part}'");
                    }
                }
                else if (double.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                {
                    numbers.Add(value);
                }
                else
                {
                    Console.WriteLine($"Warning: Could not parse '{part}' as a number");
                }
            }

            if (numbers.Count == 0)
                return string.Empty;

            // Now compress consecutive identical values
            var result = new List<string>();
            int seqCount = 1;
            double currentNumber = numbers[0];

            for (int i = 1; i < numbers.Count; i++)
            {
                if (Math.Abs(numbers[i] - currentNumber) < 0.000001) // Using small epsilon for double comparison
                {
                    seqCount++;
                }
                else
                {
                    result.Add(seqCount > 1 ? $"{seqCount}*{currentNumber.ToString(CultureInfo.InvariantCulture)}" : currentNumber.ToString(CultureInfo.InvariantCulture));
                    currentNumber = numbers[i];
                    seqCount = 1;
                }
            }

            // Add the last number or sequence
            result.Add(seqCount > 1 ? $"{seqCount}*{currentNumber.ToString(CultureInfo.InvariantCulture)}" : currentNumber.ToString(CultureInfo.InvariantCulture));

            string output = string.Join(" ", result);
            Console.WriteLine("CompressNumberSequence Output: " + output);
            return output;
        }

        // Helper method to expand coordinates with multiplier notation
        private List<double> ExpandCoordinates(string input)
        {
            List<double> result = new List<double>();
            
            if (string.IsNullOrEmpty(input))
                return result;
                
            string[] parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string part in parts)
            {
                if (part.Contains("*"))
                {
                    // Handle multiplier notation (e.g., "4*3000.00")
                    string[] multParts = part.Split('*');
                    if (multParts.Length == 2 && int.TryParse(multParts[0], out int multCount) && 
                        double.TryParse(multParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                    {
                        for (int idx = 0; idx < multCount; idx++)
                        {
                            result.Add(value);
                        }
                    }
                }
                else if (double.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                {
                    result.Add(value);
                }
            }
            
            return result;
        }

        // Function to combine labels and values back into a compressed format
        private string CombineLabelsAndValues(string labels, string values)
        {
            // Split the input strings
            string[] labelArray = labels.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] valueArray = values.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Process the values to expand any multiplier notation (e.g., "4*5000.00")
            List<string> expandedValues = new List<string>();
            foreach (string value in valueArray)
            {
                if (value.Contains("*"))
                {
                    string[] parts = value.Split('*');
                    if (int.TryParse(parts[0], out int multCount) && parts.Length == 2)
                    {
                        for (int idx = 0; idx < multCount; idx++)
                            expandedValues.Add(parts[1]);
                    }
                    else
                    {
                        expandedValues.Add(value); // If it can't be parsed properly, keep as is
                    }
                }
                else
                {
                    expandedValues.Add(value);
                }
            }
            
            // Check if lengths match after expansion
            if (labelArray.Length != expandedValues.Count)
            {
                // If not enough values, add default values to match
                while (expandedValues.Count < labelArray.Length)
                    expandedValues.Add("0.00");

                // If too many values, truncate
                if (expandedValues.Count > labelArray.Length)
                    expandedValues = expandedValues.Take(labelArray.Length).ToList();
            }
            
            // Group consecutive labels with the same value
            var result = new List<string>();
            int index = 0;
            
            while (index < labelArray.Length)
            {
                string currentLabel = labelArray[index];
                string currentValue = expandedValues[index];
                
                // Find consecutive labels with the same value
                int nextIndex = index + 1;
                StringBuilder combinedLabel = new StringBuilder(currentLabel);
                
                while (nextIndex < labelArray.Length && expandedValues[nextIndex] == currentValue)
                {
                    // Skip if the current or next label contains a period
                    if (currentLabel.Contains('.') || labelArray[nextIndex].Contains('.'))
                    {
                        // Don't combine if either has a period, but keep track of position
                        if (nextIndex == index + 1) // if we're still at the first possible combine
                            break;
                        else // we were in the middle of combining
                            break;
                    }
                    
                    combinedLabel.Append(labelArray[nextIndex]);
                    nextIndex++;
                }
                
                // Add this dimensional group to the result
                result.Add($"{combinedLabel}:{currentValue}");
                
                // Move to the next unprocessed label
                index = nextIndex;
            }
            
            return string.Join(" ", result);
        }

        // Specialized function to handle Z-axis grid labels which often use "+" prefix
        private string CombineZLabelsAndValues(string labels, string values)
        {
            // Split the input strings
            string[] labelArray = labels.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] valueArray = values.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Create a mapping of values to their corresponding labels
            Dictionary<string, string> valueToLabel = new Dictionary<string, string>();
            
            for (int i = 0; i < Math.Min(labelArray.Length, valueArray.Length); i++)
            {
                // Z-axis labels often have a "+" prefix - normalize the value for matching
                string normalizedValue = NormalizeValue(valueArray[i]);
                string normalizedLabel = labelArray[i];
                
                // Ensure the Z label has a "+" prefix if it's a numeric value
                if (!normalizedLabel.StartsWith("+") && 
                    double.TryParse(normalizedLabel.TrimStart('+'), NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                {
                    // It's a number without a "+" prefix, add one
                    normalizedLabel = "+" + normalizedLabel;
                }
                
                valueToLabel[normalizedValue] = normalizedLabel;
            }
            
            // Build the result in format "+0:0.00 +3000:3000.00"
            var result = new List<string>();
            foreach (var pair in valueToLabel)
            {
                result.Add($"{pair.Value}:{pair.Key}");
            }
            
            return string.Join(" ", result);
        }

        // Specialized function to handle Z-axis grid labels with "+" prefix in DecomposeString
        private (string, string) DecomposeZString(string input)
        {
            List<string> labels = new List<string>();
            List<string> values = new List<string>();
            
            // Split the input string into parts (e.g., "+0:0", "+3000:3000")
            string[] parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string part in parts)
            {
                // Find the position of the first colon (to handle values that might contain colons)
                int colonIndex = part.IndexOf(':');
                if (colonIndex <= 0) continue;
                
                string label = part.Substring(0, colonIndex);
                string value = part.Substring(colonIndex + 1);
                
                // For Z-axis, we keep the "+" prefix in the label but ensure the value is just the number
                if (label.StartsWith("+") && 
                    double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double numValue))
                {
                    labels.Add(label);
                    values.Add(numValue.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    // Handle normal case
                    labels.Add(label);
                    values.Add(value);
                }
            }
            
            return (string.Join(" ", labels), string.Join(" ", values));
        }

        // Helper to normalize numeric values for comparison
        private string NormalizeValue(string value)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double numValue))
                return numValue.ToString(CultureInfo.InvariantCulture);
                
            return value;
        }

        private void fillTextBoxes()
        {
            if (!model.GetConnectionStatus())
            {
                MessageBox.Show("Not connected to Tekla Structures model.", "Error");
                return;
            }

            ModelObjectEnumerator gridEnum = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.GRID);
            while (gridEnum.MoveNext())
            {
                Grid grid = gridEnum.Current as Grid;
                if (grid != null)
                {
                    try 
                    {   
                        xGridInputTextBox.Text = CombineLabelsAndValues(grid.LabelX, grid.CoordinateX);
                        yGridInputTextBox.Text = CombineLabelsAndValues(grid.LabelY, grid.CoordinateY);
                        zGridInputTextBox.Text = CombineZLabelsAndValues(grid.LabelZ, grid.CoordinateZ);
                        break; // Only process the first grid found
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error processing grid data: " + ex.Message, "Error");
                    }
                }
                else
                {
                    MessageBox.Show("No grid found in the model.", "Error");
                }
            }
        }

        // Decompose dimensions directly from input string to labels and values strings
        private (string, string) DecomposeString(string input)
        {
            List<string> labels = new List<string>();
            List<string> values = new List<string>();
            
            // Split the input string into parts (e.g., "A:0", "BC:5000.12")
            string[] parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string part in parts)
            {
                // Find the position of the first colon (to handle values that might contain colons)
                int colonIndex = part.IndexOf(':');
                if (colonIndex <= 0) continue;
                
                string combinedLabel = part.Substring(0, colonIndex);
                string value = part.Substring(colonIndex + 1);
                
                // For single character labels
                if (combinedLabel.Length == 1)
                {
                    labels.Add(combinedLabel);
                    values.Add(value);
                    continue;
                }
                
                // Process combined label (like "BC" or "C.2D")
                for (int i = 0; i < combinedLabel.Length; i++)
                {
                    // Start with current character
                    string singleLabel = combinedLabel[i].ToString();
                    
                    // If followed by a period, collect the period and all digits
                    if (i + 1 < combinedLabel.Length && combinedLabel[i + 1] == '.')
                    {
                        singleLabel += '.';
                        i += 2; // Skip past the period
                        
                        // Collect all digits
                        while (i < combinedLabel.Length && char.IsDigit(combinedLabel[i]))
                        {
                            singleLabel += combinedLabel[i];
                            i++;
                        }
                        
                        i--; // Adjust for the loop increment
                    }
                    
                    labels.Add(singleLabel);
                    values.Add(value);
                }
            }
            
            return (string.Join(" ", labels), string.Join(" ", values));
        }

        private void ModifyGridV2()
        {
            try
            {
                // Collect Grid Data
                ModelObjectEnumerator gridObjects = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.GRID);
                
                if (!gridObjects.MoveNext())
                {
                    MessageBox.Show("No grid found in the model.", "Error");
                    return;
                }
                Grid existingGrid = gridObjects.Current as Grid;
                if (existingGrid == null)
                {
                    MessageBox.Show("Failed to get grid object.", "Error");
                    return;
                }

                // Process the input to extract labels and coordinates
                var xGridData = DecomposeString(xGridInputTextBox.Text);
                var yGridData = DecomposeString(yGridInputTextBox.Text);
                var zGridData = DecomposeZString(zGridInputTextBox.Text); // Use Z-specific parsing

                // Update the grid properties
                existingGrid.LabelX = xGridData.Item1;
                existingGrid.CoordinateX = xGridData.Item2;
                
                existingGrid.LabelY = yGridData.Item1;
                existingGrid.CoordinateY = yGridData.Item2;
                
                existingGrid.LabelZ = zGridData.Item1;
                existingGrid.CoordinateZ = zGridData.Item2;

                // Modify the grid
                bool result = existingGrid.Modify();

                // Commit changes to model
                model.CommitChanges("Modified grid");

                // Refresh the textboxes with the new grid data
                fillTextBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error modifying grids: " + ex.Message, "Error");
            }
        }

        private void DeleteAllGrids()
        {
            ModelObjectEnumerator grids = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.GRID);
            while (grids.MoveNext())
            {
                ModelObject grid = grids.Current;
                if (grid != null)
                    grid.Delete();
            }
        }
        
        // Add this method for handling the Apply Custom Grid button click
        private void CreateGridbtn_Click(object sender, EventArgs e)
        {
            ModifyGridV2();
        }
    }
}

