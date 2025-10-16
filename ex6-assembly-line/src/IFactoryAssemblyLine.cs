// This file defines the IFactoryAssemblyLine interface, which outlines the methods that the FactoryAssemblyLine class must implement.
public interface IFactoryAssemblyLine
{
  /// <summary>
  /// Adds a station with the specified ID and processing time.
  /// </summary>
  /// <param name="stationId">The ID of the station.</param>
  /// <param name="processingTime">The processing time of the station.</param>
  void AddStation(int stationId, int processingTime);

  /// <summary>
  /// Removes the station with the specified ID.
  /// </summary>
  /// <param name="stationId">The ID of the station.</param>
  void RemoveStation(int stationId);

  /// <summary>
  /// Activates the station with the specified ID.
  /// </summary>
  /// <param name="stationId">The ID of the station.</param>
  void StartAssembly(int stationId);

  /// <summary>
  /// Deactivates the station with the specified ID.
  /// </summary>
  /// <param name="stationId">The ID of the station.</param>
  void StopAssembly(int stationId);

  /// <summary>
  /// Returns the processing time of the specified station.
  /// </summary>
  /// <param name="stationId">The ID of the station.</param>
  /// <returns>The processing time of the station.</returns>
  int GetProcessingTime(int stationId);

  /// <summary>
  /// Returns the total processing time of all active stations.
  /// </summary>
  /// <returns>The total processing time.</returns>
  int GetTotalProcessingTime();

  /// <summary>
  /// Returns the total number of stations.
  /// </summary>
  /// <returns>The total number of stations.</returns>
  int GetNumStations();

  /// <summary>
  /// Returns the number of active stations.
  /// </summary>
  /// <returns>The number of active stations.</returns>
  int GetNumActiveStations();

  /// <summary>
  /// Returns the number of inactive stations.
  /// </summary>
  /// <returns>The number of inactive stations.</returns>
  int GetNumInactiveStations();

  /// <summary>
  /// Returns true if the specified station is active, otherwise false.
  /// </summary>
  /// <param name="stationId">The ID of the station.</param>
  /// <returns>True if the station is active, otherwise false.</returns>
  bool IsStationActive(int stationId);
}