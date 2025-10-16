using System;
using System.Collections.Generic;


public class FactoryAssemblyLine : IFactoryAssemblyLine
{
  // Capacity provided at construction (total allowed stations). Counting semantics to be implemented.
  private readonly int _capacity;
  // Storage for added stations (students will decide on station representation & counters).
  private readonly Dictionary<int, Station> _stations;

  /// <summary>
  /// Creates a new assembly line with a fixed capacity.
  /// </summary>
  /// <param name="numStations">Total station capacity (must be &gt;= 0).</param>
  /// <exception cref="ArgumentException">If capacity is negative.</exception>
  public FactoryAssemblyLine(int numStations)
  {
    if (numStations < 0)
    {
      throw new ArgumentException("Number of stations cannot be negative", nameof(numStations));
    }
    _capacity = numStations;
    _stations = new Dictionary<int, Station>();
  }

  //Implement per specs.md (validation, capacity logic, counts, idempotent start/stop).
  public void AddStation(int stationId, int processingTime) => throw new NotImplementedException();
  public void RemoveStation(int stationId) => throw new NotImplementedException();
  public void StartAssembly(int stationId) => throw new NotImplementedException();
  public void StopAssembly(int stationId) => throw new NotImplementedException();
  public int GetProcessingTime(int stationId) => throw new NotImplementedException();
  public int GetTotalProcessingTime() => throw new NotImplementedException();
  public int GetNumStations() => throw new NotImplementedException();
  public int GetNumActiveStations() => throw new NotImplementedException();
  public int GetNumInactiveStations() => throw new NotImplementedException();
  public bool IsStationActive(int stationId) => throw new NotImplementedException();

  // Placeholder station model (students may refactor to record / sealed class as desired).
  private class Station
  {
    public int ProcessingTime { get; set; }
    public bool IsActive { get; set; }
  }
}