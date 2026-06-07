## Smart Home Energy Dashboard

C# .NET MAUI application designed to monitor, simulate, and analyze home energy consumption and production. 
The application utilizes an isolated Data Access Layer (DAL) powered by Entity Framework Core 
and SQLite to manage data efficiently.

### Functional Requirements

The user will be able to:

- Manage Devices (CRUD operations): Perform full Create, Read, Update, 
  and Delete operations where applicable to add and configure home appliances 
  (consumers) and green energy sources (producers).
- Run Real-Time Simulation: Start and stop a real-time simulation engine that dynamically generates data 
  regarding current power production and consumption.
- Monitor Battery Status: Visualize the live state of charge (SoC) of a virtual home battery storage system, 
  tracking whether energy is currently being stored (charging) or drawn (discharging).
- Log Simulation History: Automatically record and log detailed energy data into separate  
  historical databases during the simulation run.
- View Charts and Statistics: Display aggregated graphs and summary statistics comparing energy production versus consumption.
- Export and Import Data: Export the recorded measurement history into a JSON file format 
  and re-import it back into the application for further analysis.