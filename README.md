# Grid Config Tool

-- older versions of tekla plugins I have made for work. In updated version I have made these for tekla versions 2024+ in a wpf ui format -- 

## Overview
The Grid Config Tool is a simple utility that allows you to modify Tekla Structures grid labels and coordinates through a user-friendly interface. This tool helps streamline the process of updating grids in your Tekla models.

## Installation Instructions

### Downloading from GitHub (No Account Required)
1. Go to the GitHub repository URL you received
2. Click the green "Code" button near the top-right of the page
3. Select "Download ZIP" from the dropdown menu
4. Save the ZIP file to a location on your computer
5. Right-click the downloaded ZIP file and select "Extract All..."
6. Choose a folder location and click "Extract"
7. Navigate to the extracted folder, then to the `bin/x64/Debug` directory
8. Double-click `GridConfig.exe` to run the application

### Requirements
- Windows operating system
- Tekla Structures installed on your computer
- An open Tekla Structures model

## How to Use

1. **Start Tekla Structures** and open your model
2. **Launch GridConfig.exe** from the extracted folder
3. **Select a grid** in your Tekla model by clicking on it
4. The application will display the current grid labels and coordinates
5. **Modify the values** in the text boxes:
   - X Grid: Modify the X-axis grid labels and coordinates
   - Y Grid: Modify the Y-axis grid labels and coordinates
   - Z Grid: Modify the Z-axis grid labels and coordinates
6. **Press Enter** in any text box to apply your changes
7. The grid in the Tekla model will update immediately

## Format Examples

When entering grid values, use the following format:
- X and Y coordinates: `A:0 BC:1000 D:2000.95 D.5:3000 E:5000`
- Z coordinates: `+0:0.00,+5000:5000 +7000:7000.11`

## Troubleshooting

- **Application doesn't connect to Tekla**: Make sure Tekla Structures is open with a model loaded before launching the Grid Config Tool
- **Changes don't apply**: Ensure you press Enter after modifying values in the text boxes
- **Grid not showing in the application**: Make sure you have selected a grid object in the Tekla model

## Support
If you encounter any issues:
1. Make sure you've followed all installation steps correctly
2. Check the troubleshooting section above
3. For technical assistance, please contact the developer directly with:
   - A description of what you were trying to do
   - Any error messages you received
   - A screenshot of the problem (if possible)

This tool was developed to simplify grid configuration in Tekla Structures models. 
