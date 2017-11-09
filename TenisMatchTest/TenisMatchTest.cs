using System;
using NUnit.Framework;
using TenisMatch.Class;

namespace TenisMatchTest
{

    /// <summary>
    /// Simple Test class
    /// </summary>
    [TestFixture]
    public class TenisMatchTest
    {
         
        protected Partido PartidoInitializate
        {
            get
            {
                //Inicializa un partido, tipico
                Partido partido = new Partido()
                {
                    NumerodeSets = PartidoSets.Partido3Sets,
                    JugadorSaca = JugadorEnum.Player1,
                };

                partido.AddJugador(new Jugador("Jugador1"));
                partido.AddJugador(new Jugador("jugador2"));

                return partido;
            }


        }

        /// <summary>
        /// Testeamos casos en los que el inicio del partido deberia fallar
        /// </summary>
        //Sin jugadores
        [TestCase(null, null, null, JugadorEnum.Player1, PartidoSets.Partido5Sets)]
        //Con 1 jugador
        [TestCase("uno", "", "", JugadorEnum.Player1, PartidoSets.Partido5Sets)]
        //Con 3 jugadores
        [TestCase("uno","dos", "tres", JugadorEnum.Player1, PartidoSets.Partido5Sets)]
        //Sin Sets
        [TestCase("uno", "dos", null, JugadorEnum.Player1, PartidoSets.None)]
        public void CheckValidacion(string user1,string user2,string user3, JugadorEnum? jugador,PartidoSets? sets)
        {

            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                IPartidoTenis partido = new Partido();
                if (user1 != null)
                    partido.AddJugador(new Jugador(user1));

                if (user2 != null)
                    partido.AddJugador(new Jugador(user2));

                if (user3 != null)
                    partido.AddJugador(new Jugador(user3));

                if (sets.HasValue)
                    partido.NumerodeSets = sets.Value;

                if (jugador.HasValue)
                    partido.JugadorSaca = jugador.Value;


                partido.ComienzaPartido();
            });

        }


        [Test]
        /// <summary>
        /// Comprobacion de que la estructura de clases funciona correctamente
        /// </summary>
        public void CheckSchema()
        {
            bool comienzo = false;


            Partido partido = PartidoInitializate;
            //Check partido no jugado

            Assert.IsTrue(partido.Estado == Estado.NoJugado);


            //Definimos callbacks para controlar que saltan
            ((IPartidoTenis)partido).ComienzaPartidoCallback(p => { comienzo = true; });

            ((IPartidoTenis) partido).ComienzaPartido();
            Assert.IsTrue(comienzo, "No salto el evento ComienzoPartido");

            
            //Check partido en juego
            Assert.IsTrue(partido.Estado == Estado.EnJuego);

            partido.ComienzaPunto();
            
            //Check 1 set en el partido, y es el set 'Enjuego'
            Assert.AreEqual(1, partido.Sets.Count);
            Assert.AreEqual(partido.EnJuego, partido.Sets[0]);

            //Check 1 Juego en el set, y es el q esta en Juego
            Assert.AreEqual(1, partido.Sets[0].Juegos.Count);
            Assert.AreEqual(partido.Sets[0].EnJuego, partido.Sets[0].Juegos[0]);

            //Check 1 Punto en el Juego, y es el que esta en Juego
            Assert.AreEqual(1, partido.Sets[0].Juegos[0].Puntos.Count);
            Assert.AreEqual( partido.Sets[0].Juegos[0].Puntos[0], partido.Sets[0].Juegos[0].EnJuego);


            partido.GanaPunto(JugadorEnum.Player1);



        }

        [Test]
        /// <summary>
        /// Comprobacion de que la puntuacion del Juego funciona OK
        /// </summary>
        public void CheckPuntuacionJuego()
        {
            
            bool comienzapunto = false;
            bool ganapunto = false;
            bool comienzajuego = false;
            bool ganajuego = false;
            Partido partido = PartidoInitializate;

            //Definimos callbacks para controlar que saltan
            
            ((IPartidoTenis)partido).ComienzaPuntoCallback(p => { comienzapunto = true; });
            ((IPartidoTenis)partido).GanaPuntoCallback(p => { ganapunto = true; });

            ((IPartidoTenis)partido).ComienzaJuegoCallback(p => { comienzajuego = true; });
            ((IPartidoTenis)partido).GanaJuegoCallback(p => { ganajuego = true; });


            ((IPartidoTenis)partido).ComienzaPartido();



            partido.ComienzaPunto();


            //Deberia haber saltado el evento comienzoJuego
            Assert.IsTrue(comienzapunto, "No salto el evento comienzo punto");
            Assert.IsTrue(comienzajuego, "No salto el evento comienzo juego");

            Juego juego = partido.Sets[0].Juegos[0];
            
            partido.GanaPunto(JugadorEnum.Player1);
            Assert.AreEqual(juego.PuntuacionTenisJugador1, PuntuacionJuego.Quince);

            
            partido.ComienzaPunto();
            partido.GanaPunto(JugadorEnum.Player1);
            Assert.AreEqual(juego.PuntuacionTenisJugador1, PuntuacionJuego.Treinta);


            partido.ComienzaPunto();
            partido.GanaPunto(JugadorEnum.Player1);
            Assert.AreEqual(juego.PuntuacionTenisJugador1, PuntuacionJuego.Cuarenta);

            partido.ComienzaPunto();
            partido.GanaPunto(JugadorEnum.Player1);
            Assert.AreEqual(juego.PuntuacionTenisJugador1, PuntuacionJuego.Ganado);

            //Si ha ganado el juego se da por terminado
            Assert.AreEqual(juego.Estado, Estado.Terminado);

            //Deberia haber saltado el evento ganaJuego
            Assert.IsTrue(ganapunto, "No salto el evento gana punto");
            Assert.IsTrue(ganajuego, "No salto el evento gana juego");
        }

        [Test]
        /// <summary>
        /// Comprobacion de que la puntuacion del Juego funciona con caso de DEUCE
        /// </summary>
        public void CheckPuntuacionJuegoDeuce()
        {

        
            Partido partido = PartidoInitializate;
            ((IPartidoTenis)partido).ComienzaPartido();

            //Los ponemos a 40.. La propiedad Puntuacion del Juego esta inaccesible (internal set)
            for (int i=0;i<3;i++)
            {
                //Gana Punto Player1
                partido.ComienzaPunto();
                partido.GanaPunto(JugadorEnum.Player1);

                //Gana Punto Player2
                partido.ComienzaPunto();
                partido.GanaPunto(JugadorEnum.Player2);
            }

            var juego = partido.Sets[0].Juegos[0];
            Assert.AreEqual(juego.PuntuacionTenisJugador1, PuntuacionJuego.Cuarenta);
            Assert.AreEqual(juego.PuntuacionTenisJugador2, PuntuacionJuego.Cuarenta);

            //Si gana player2 se pone con ventaja
            partido.ComienzaPunto();
            partido.GanaPunto(JugadorEnum.Player2);
            Assert.AreEqual(juego.PuntuacionTenisJugador2, PuntuacionJuego.Ventaja);

            //Si gana player1 vuelven a 40/40
            partido.ComienzaPunto();
            partido.GanaPunto(JugadorEnum.Player1);
            Assert.AreEqual(juego.PuntuacionTenisJugador1, PuntuacionJuego.Cuarenta);
            Assert.AreEqual(juego.PuntuacionTenisJugador2, PuntuacionJuego.Cuarenta);

            //Si gana player1  se pone con ventaja
            partido.ComienzaPunto();
            partido.GanaPunto(JugadorEnum.Player1);
            Assert.AreEqual(juego.PuntuacionTenisJugador1, PuntuacionJuego.Ventaja);


            //Si gana player1 gana
            partido.ComienzaPunto();
            partido.GanaPunto(JugadorEnum.Player1);
            Assert.AreEqual(juego.PuntuacionTenisJugador1, PuntuacionJuego.Ganado);
            Assert.AreEqual(juego.Estado, Estado.Terminado);


        }

        [Test]
        /// <summary>
        /// DEtermina si serializa correctamente la enumeracion
        /// </summary>
        public void CheckSerializacionEnum()
        {
            PuntuacionJuego puntuacion = PuntuacionJuego.Ganado;
            Assert.AreEqual(puntuacion.ToDescription(), "Win");
        }


        [Test]
        /// <summary>
        /// Comprobacion de que la puntuacion del SEt funciona OK
        /// </summary>
        public void CheckPuntuacionSet()
        {
             //TO-DO
        }
        
        

        ///
        /// Por hacer N --tests..
        ///






    }
}
