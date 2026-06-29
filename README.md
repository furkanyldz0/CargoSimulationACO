# ACO-Dijkstra Cargo Simulation

<p align="center">
  <img src="images/introduction.jpg" alt="ACO-Dijkstra Cargo Simulation">
  <br>
</p>

This project is a cargo delivery simulation based on ACO-Dijkstra algorithms. It takes approach to shortest path finding problem on city map that consists of 24 city, in other words: nodes. Each city has multiple neighbour cities and roads(i.e. edge - back and forth) that connect each other.

<p align="center">
  <img src="images/map.jpg" alt="City Map">
  <br>
  <em>City map</em>
</p>

## How does it work?

The observer selects a start and target city by order. After selection is complete, observer initiates a simulation run.

<p align="center">
  <img src="images/selection-idle.jpg" alt="Before selection">
  <br>
  <em>Before selection</em>
</p>

<p align="center">
  <img src="images/selection-start.jpg" alt="Selecting start">
  <br>
  <em>Selecting start city</em>
</p>

<p align="center">
  <img src="images/selection-target.jpg" alt="Selecting target">
  <br>
  <em>Selecting target city</em>
</p>

During the run, vehicles start being generate at start city and travel to cities across the map with ACO algorithm until they find the target city. Once find the target they leave pheromone substance on the roads that used during traveling to target and return to the start city, using dijkstra algorithm.

<p align="center">
  <img src="images/run-start.jpg" alt="First situation of run">
  <br>
  <em>First situation of run, cargo vehicles are discovering the route</em>
</p>

<p align="center">
  <img src="images/run-final.jpg" alt="Final situation">
  <br>
  <em>Final situtation, most cargo vehicles have learned the shortest route</em>
</p>

Dijkstra computes the shortest path from target to start before the initiation of run and has nothing to do with pheromones. ACO in other hands, it does not know the shortest path and tries to learn it with discovery, considering pheromones and road distances. The run stops when the max vehicle count is reached, or can be stopped by observer's desire any moment. All runs are being recorded and can be monitored with statistics panel.

<p align="center">
  <img src="images/statistics-panel.jpg" alt="Statistics Panel">
  <br>
  <em>Statistics panel: Dijkstra route, ACO/Dijkstra ratios, ACO converged and best route, Alpha-Beta-Evaporation rate-Q values</em>
</p>

The observer can change all cargo vehicle's behaviour with vehicle and ACO parameters; these settings affect the herd behaviour.

## Vehicle settings

* Vehicle spawn interval
* Vehicle speed
* Vehicle count
* Time scale
* Pheromone trails

<p align="center">
  <img src="images/vehicle-settings.jpg" alt="Vehicle Settings">
  <br>
  <em>Vehicle settings</em>
</p>

## ACO settings

* Pheromone weight (alpha)
* Distance weight (beta)
* Initial pheromone level
* Minimum pheromone amount
* Pheromone evaporation rate
* Q value

<p align="center">
  <img src="images/aco-settings.jpg" alt="ACO Settings">
  <br>
  <em>ACO settings</em>
</p>

---

If you have any questions about project, contact me via linkedin (furkanyldz0).