Algunas consideraciones:
===================================
La solucion se realizo con VS2015.

El proyecto de Test esta implementado utilizando NUnit.

Asi que para ejecutar los test desde VStudio hay que instalar el plugin NUnitTestAdapter.

Diseño & Analisis
===================================
La foto de objetos que intervienen la app es la siguiente.
```
Partido 
 -> Sets 
		-> Juegos
				-> Puntos
 -> Jugadores
```
Para ver diseño de clases puede ver el archivo ClassDiagram.cd incluido en el proyecto.

La clase principal es la clase Partido, es la clase en la cual estan contenidos el resto de objetos de la app.

-Todas las clases contenedores(colecciones)heredan de Items<> excepto Jugadores por su simplicidad. 

-La clase Puntos hereda de la clase ItemBase.
-Las clases Partidos/Sets/Juegos heredan de un hijo de esta ya que tambien comparten estas propiedades

  Estado (Identifica si el (partido/set/juego/punto) esta en juego ,terminado o sin iniciar
	Puntuacionacion actual ((partido/set/juego/punto) de cada jugador 
	En el caso que haya terminado se almacena el Jugador que ha ganado.

Los objetos	 (Partidos/Sets/Juegos) ademas son contenedores de otras clases (Partidos de sets, Sets de juegos, Juegos de puntos) por eso 
heredan de otra clase Item que a su vez hereda de ItemBase
El ciclo de vida de estos objetos(partido/set/juego)son similares y se representan en la tabla con estos dos metodos:
```
	->Comenzar()
		->OnComienza() ->  metodo que lanza callbacks
	->Ganar()
		->OnGanar() - > metodo que lanza callbacks
	->HaGanado() -> logica para determinar si ha ganado  (partido/set/juego)
```

-La clase Jugador es muy simple y solo tiene propiedades getters/setters y la herencia es simple.

Se usan tres enumeraciones :

-**PuntuacionJuego**: Se adapta la enumeracion al modo en que se contabilizan los puntos en el tenis (0-15-30-40-Ad-Win). Se ha creado un metodo de extension de metodos para obtener la descripcion de esta enumeracion

-**Estado**: Estado de un item 

-**JugadorEnum**: Indicador de jugador (Player1-player2)


Coding
===================================
Se ha primado la orientacion a objetos, empleando generics para que los metodos al heredar sean tipados.
Todos metodos estan con sus correspondientes comentarios.

Testing
===================================
Se han realizado pruebas sencillas, pero por falta de tiempo su alcance no ofrece cobertura para todo el código. 
Mock y DI han sido descartados por las caracteristicas del proyecto.



	 
