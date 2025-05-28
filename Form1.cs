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
using TSM = Tekla.Structures.Model;
using TSMU = Tekla.Structures.Model.UI;

namespace GridConfigV2
{
    public partial class Form1 : Form
    {
        private Model model;
        private TSM.Events _events;
        private Grid _selectedGrid;
        
        public Form1()
        {
            InitializeComponent();
            model = new Model();
            _events = new TSM.Events();
            
            // Register events
            this.Load += Form1_Load;
            this.FormClosed += Form1_FormClose;
            
            // Add KeyDown event handlers to textboxes
            xGridInputTextBox.KeyDown += GridInputTextBox_KeyDown;
            yGridInputTextBox.KeyDown += GridInputTextBox_KeyDown;
            zGridInputTextBox.KeyDown += GridInputTextBox_KeyDown;
        }

        private void Events_SelectionChange()
        {
            try 
            {
                // Always use BeginInvoke for event handlers that might be called from non-UI threads
                this.BeginInvoke(new MethodInvoker(() => { UpdateSelectedGridInfo(); }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in selection change event: {ex.Message}", "Error");
            }
        }

        private void UpdateSelectedGridInfo()
        {
            try
            {
                // Check if we need to invoke on the UI thread
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(UpdateSelectedGridInfo));
                    return;
                }

                // Get the selected objects
                TSMU.ModelObjectSelector selector = new TSMU.ModelObjectSelector();
                ModelObjectEnumerator selectedObjects = selector.GetSelectedObjects();

                // Check if there is exactly one selected object
                _selectedGrid = null;
                while (selectedObjects.MoveNext())
                {
                    if (selectedObjects.Current is Grid grid)
                    {
                        _selectedGrid = grid;
                        break;
                    }
                }

                // Update the textboxes with the selected grid data
                if (_selectedGrid != null)
                {
                    xGridInputTextBox.Text = StringFormatHandling.CombineLabelsAndValues(_selectedGrid.LabelX, _selectedGrid.CoordinateX);
                    yGridInputTextBox.Text = StringFormatHandling.CombineLabelsAndValues(_selectedGrid.LabelY, _selectedGrid.CoordinateY);
                    zGridInputTextBox.Text = StringFormatHandling.CombineZLabelsAndValues(_selectedGrid.LabelZ, _selectedGrid.CoordinateZ);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in UpdateSelectedGridInfo: {ex.Message}", ex);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!model.GetConnectionStatus())
            {
                MessageBox.Show("Not connected to Tekla Structures model.", "Connection Error");
                return;
            }

            _events.SelectionChange += Events_SelectionChange;
            _events.Register();
            
            // Check if there's already a grid selected when the form loads
            UpdateSelectedGridInfo();
            
            // Also register for form activation to update when the form regains focus
            this.Activated += Form1_Activated;
        }
        
        private void Form1_Activated(object sender, EventArgs e)
        {
            // The Activated event should already be on the UI thread, but use try-catch for safety
            try
            {
                // Update the selection when the form is activated
                UpdateSelectedGridInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating data on form activation: {ex.Message}", "Error");
            }
        }

        private void Form1_FormClose(object sender, FormClosedEventArgs e)
        {
            if (_events != null)
            {
                _events.SelectionChange -= Events_SelectionChange;
                _events.UnRegister();
            }
            
            // Unregister the form activation event
            this.Activated -= Form1_Activated;
        }

        private void ModifyGrid()
        {
            try
            {
                // Check if we have a selected grid
                if (_selectedGrid == null)
                {
                    MessageBox.Show("No grid is currently selected. Please select a grid in the model first.", "No Selection");
                    return;
                }

                // Process the input to extract labels and coordinates
                var xGridData = StringFormatHandling.DecomposeString(xGridInputTextBox.Text);
                var yGridData = StringFormatHandling.DecomposeString(yGridInputTextBox.Text);
                var zGridData = StringFormatHandling.DecomposeZString(zGridInputTextBox.Text); // Use Z-specific parsing

                // Update the grid properties
                _selectedGrid.LabelX = xGridData.Item1;
                _selectedGrid.CoordinateX = xGridData.Item2;
                
                _selectedGrid.LabelY = yGridData.Item1;
                _selectedGrid.CoordinateY = yGridData.Item2;
                
                _selectedGrid.LabelZ = zGridData.Item1;
                _selectedGrid.CoordinateZ = zGridData.Item2;

                // Modify the grid
                bool result = _selectedGrid.Modify();

                // Commit changes to model
                model.CommitChanges("Modified grid");

                // Refresh the textboxes with the new grid data
                UpdateSelectedGridInfo();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error modifying grid: {ex.Message}", "Error");
            }
        }

        private void GridInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if Enter key was pressed
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;  // Prevent the "ding" sound
                e.SuppressKeyPress = true;  // Prevent default behavior (like adding a new line)
                
                // Apply the grid changes
                ModifyGrid();
            }
        }
    }
}

