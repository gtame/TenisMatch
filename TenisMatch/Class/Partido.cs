using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenisMatch.Class
{
    /// <summary>
    /// Representa el Partido, es la clase de primer nivel
    /// </summary>
    public class Partido : Item<Partido,Set,Sets>, IPartidoTenis
    {
        #region Vars Member
        private Jugadores _ljugadores; //Jugadores del partido
        private PartidoSets _numsets; //Numero de sets a disputar
        private Random _rnd; //Generador de num aleatorios
        private JugadorEnum _jugadorsaca; //Jugador que realiza el saca

        #region Callbacks
        protected Action<IPartidoTenis> newcomienzoCallback;
        protected Action<IPartidoTenis> newganaCallback;
        protected Action<Punto> comienzoPuntocallback;
        protected Action<Punto> ganaPuntocallback;
        protected Action<Juego> comienzoJuegocallback;
        protected Action<Juego> ganaJuegocallback;
        protected Action<Set> comienzoSetcallback;
        protected Action<Set> ganaSetcallback;
        #endregion

        #endregion

        #region Constructor
        public Partido()
        {
            //Inicializamos  jugadores
            _ljugadores = new Jugadores();
            //variable de random
            _rnd = new Random();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Jugador seleccionado para el saque
        /// </summary>
        [DefaultValue(JugadorEnum.None)]
        public JugadorEnum JugadorSaca
        {
            get { return _jugadorsaca; }
            set
            {
                if (Estado != Estado.NoJugado)
                    throw new System.InvalidOperationException("Solo se puede indicar si no se incio el partido");

                _jugadorsaca = value;
            }
        }

        /// <summary>
        /// Set actual que se esta jugando
        /// </summary>
        public Set SetActual
        {
            get
            {
                return EnJuego;
            }
        }

        /// <summary>
        /// Juego que se esta disputando actualmente en el partido.
        /// </summary>
        public Juego JuegoActual
        {
            get
            {
                if (SetActual != null)
                    return SetActual.EnJuego;
                else
                    return null;
            }
        }


        /// <summary>
        /// Punto  ACtual en Juego en el partido
        /// </summary>
        public Punto PuntoActual
        {
            get
            {
                if (JuegoActual != null)
                    return JuegoActual.EnJuego;
                else
                    return null;
            }

        }


        /// <summary>
        ///Numero de Sets
        /// </summary>
        [DefaultValue(PartidoSets.None)]
        public PartidoSets NumerodeSets
        {
            get
            {
                return _numsets;
            }
            set
            {
                if (Estado != Estado.NoJugado && value != _numsets)
                    throw new InvalidOperationException("No se puede fijar el numero de partidos, esto debe realizarse antes de jugar el partido");

                _numsets = value;
            }
        }


        /// <summary>
        /// Jugadores que disputan el partido
        /// </summary>
        public IReadOnlyList<Jugador> Jugadores
        {
            get
            {
                return this._ljugadores.AsReadOnly();
            }
        }


        /// <summary>
        /// Sets disputados o en juego
        /// </summary>
        public IReadOnlyList<Set> Sets
        {
            get
            {
                return this.Items.AsReadOnly();
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Añade un jugador al partido
        /// </summary>
        /// <param name="jugador"></param>
        public void AddJugador(Jugador jugador)
        {
            if (_ljugadores.Count == 2)
                throw new InvalidOperationException("No se pueden añadir más de dos jugadores al partido");

            _ljugadores.Add(jugador);

            //Asignando Id evitamos que se lie por nombre
            jugador.Id = (JugadorEnum)_ljugadores.Count;
        }

        
        /// <summary>
        /// Selecciona al azar un jugador
        /// </summary>
        /// <returns></returns>
        public JugadorEnum GetJugadorRandom()
        {
            return (JugadorEnum)_rnd.Next(1, _ljugadores.Count+1);
        }

        /// <summary>
        /// Llamada a funcion para incializar partido
        /// </summary>
        public void ComienzaPartido()
        {

            //Validamos
            #region Validamos
            if (_ljugadores.Count != 2)
                throw new InvalidOperationException("Deben estar asignados los jugadores");

            if (NumerodeSets == PartidoSets.None)
                throw new InvalidOperationException("No especificado el numero de sets");

            if (Estado != Estado.NoJugado)
                throw new InvalidOperationException("El partido ya fue iniciado");


            if (JugadorSaca == JugadorEnum.None)
                throw new InvalidOperationException("No ha definido quien saca");

            #endregion

            //Cambiamos el estado
            Estado = Estado.EnJuego;

            //Lanzamos callback
            if (newcomienzoCallback != null)
                newcomienzoCallback(this);
        }



        /// <summary>
        /// Funcion para el comienzo de un nuevo punto
        /// </summary>
        /// <returns></returns>
        public Punto ComienzaPunto()
        {
            if (Estado != Estado.EnJuego)
                throw new InvalidOperationException("El partido no esta activo");

            if (PuntoActual != null)
                throw new InvalidOperationException("Hay un punto en juego");


            //Comienza Set1
            if (SetActual == null)
            {
                Set set = new Set();
                set.ComienzoCallback(comienzoSetcallback);
                set.GanaCallback(ganaSetcallback);
                Comenzar(set);
            }

            if (JuegoActual == null)
            {
                //Comienza Juego
                Juego juego = new Juego();
                juego.ComienzoCallback(comienzoJuegocallback);
                juego.GanaCallback(ganaJuegocallback);
                SetActual.Comenzar(juego);
            }


            Punto punto = new Punto();
            punto.ComienzoCallback(comienzoPuntocallback);
            punto.GanaCallback(ganaPuntocallback);
            JuegoActual.Comenzar(punto);



            return punto;

        }


        /// <summary>
        /// Llamada para indicar quien gana el punto en juego
        /// </summary>
        /// <param name="jugador">Jugador que gana</param>
        public void GanaPunto(JugadorEnum jugador)
        {
         
            //Check partido en juego
            if (Estado != Estado.EnJuego)
                throw new InvalidOperationException("El partido no esta activo");

            //Check que hay en juego punto actual
            if (PuntoActual == null)
                throw new InvalidOperationException("No hay punto en juego, debe llamar a comienza Punto");

            //Asignamos ganadores y perdores
            PuntoActual.JugadorGana = jugador;
            PuntoActual.JugadorPierde = (PuntoActual.JugadorGana == JugadorEnum.Player1 ? JugadorEnum.Player2 : JugadorEnum.Player1);

            JuegoActual.Ganar(PuntoActual);

        }



        /// <summary>
        /// Muestra los resultados de los sets del partido  Ej.- (5-2, 0-0, 0-0)
        /// Incluso los no jugados 0-0
        /// </summary>
        /// <returns></returns>
        public string ToResultString()
        {
            string result = string.Empty;

            if (NumerodeSets == PartidoSets.Partido3Sets)
                return string.Format("({0}, {1}, {2})", GetSetString(0), GetSetString(1), GetSetString(2));
            else if (NumerodeSets == PartidoSets.Partido5Sets)
                return string.Format("({0}, {1}, {2}, {3}, {4})", GetSetString(0), GetSetString(1), GetSetString(2), GetSetString(3), GetSetString(4));
            else
                return string.Empty;

        }


        /// <summary>
        /// Metodo que devuelve el resultado del set , si todavia no se ha jugado devuelve 0-0
        /// </summary>
        /// <param name="index">Indice del Set jugado, en base 0</param>
        /// <returns>Ej.5-3</returns>
        private string GetSetString(int index)
        {
            if (index < Sets.Count)
                return Sets.ElementAt(index).ToResultString();
            else
                return Set.ZERO_STRING;
        }



        #region Overrides
        /// <summary>
        /// Es invocado cuando un Set es ganado por un jugador
        /// </summary>
        /// <param name="item"></param>
        public override void Ganar(Set item)
        {
            if (item.JugadorGana == JugadorEnum.Player1)
                PuntosJugador1++;
            else
                PuntosJugador2++;

            base.Ganar(item);
        }


        /// <summary>
        /// Metodo invocado cuando se gana el partido
        /// </summary>
        /// <param name="action"></param>
        protected override void OnGana(Action<Partido> action)
        {
            base.OnGana(action);

            if (newganaCallback != null)
                newganaCallback(this);
        }


        /// <summary>
        /// Invocado cuando se comienza un nuevo set
        /// </summary>
        /// <param name="item"></param>
        public override void Comenzar(Set item)
        {
            item.Partido = this;
            base.Comenzar(item);

        }


        /// <summary>
        /// Calculo que se llama cada vez que un jugador gana un nuevo partido
        /// </summary>
        /// <param name="item">set ganado</param>
        /// <returns>true si gano el partido, false partido continua</returns>
        protected override bool HaGanado(Set item)
        {
            //A 3 sets -> Si uno de los jugadores gano 2 sets-Gana el partido
            if (NumerodeSets==PartidoSets.Partido3Sets)
                return (PuntosJugador1 == 2 || PuntosJugador2 == 2);
            //A 5 sets -> Si uno de los jugadores gano 3 sets-Win
            else if (NumerodeSets == PartidoSets.Partido5Sets)
                return (PuntosJugador1 == 3 || PuntosJugador2 == 3);
            else
                return false;
        }


        /// <summary>
        /// Metodo que es llamado cuando comienza un partido para lanzar callbacks
        /// </summary>
        /// <param name="action"></param>
        protected override void OnComienzo(Action<Partido> action)
        {
            base.OnComienzo(action);

        }



        /// <summary>
        /// Serializa a string segun requerimientos
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Estado == Estado.Terminado)
            {
                Jugador ganador = Jugadores.First(j => j.Id == JugadorGana);
                return (string.Format("\r\n!!{0} GANA EL PARTIDO¡¡", ganador.Nombre.ToUpper()));
            }
            //Todavia no jugado
            else
                return string.Empty;
        }

        #endregion


        #region IPartidoTenis Callbacks

        #region Comienzo Callbacks
        /// <summary>
        /// Callback inicio partido
        /// </summary>
        /// <param name="callback"></param>
        void IPartidoTenis.ComienzaPartidoCallback(Action<IPartidoTenis> callback)
        {
            newcomienzoCallback += callback;
    
        }

        /// <summary>
        /// Callback inicio Juego 
        /// </summary>
        /// <param name="callback"></param>
        void IPartidoTenis.ComienzaJuegoCallback(Action<Juego> callback)
        {
            comienzoJuegocallback += callback;
        }

        /// <summary>
        /// Callback inicio Set
        /// </summary>
        /// <param name="callback"></param>
        void IPartidoTenis.ComienzaSetCallback(Action<Set> callback)
        {
            comienzoSetcallback += callback;
        }

        /// <summary>
        /// Callback inicio Punto
        /// </summary>
        /// <param name="callback"></param>
        void IPartidoTenis.ComienzaPuntoCallback(Action<Punto> callback)
        {
            comienzoPuntocallback += callback;
        }

        #endregion


        #region Gana Callbacks
        /// <summary>
        /// Callback gana de puntos
        /// </summary>
        /// <param name="callback"></param>
        void IPartidoTenis.GanaPuntoCallback(Action<Punto> callback)
        {
            ganaPuntocallback += callback;
        }

        /// <summary>
        /// Callback gana Juego
        /// </summary>
        /// <param name="callback"></param>
        void IPartidoTenis.GanaJuegoCallback(Action<Juego> callback)
        {
            ganaJuegocallback += callback;

        }

        /// <summary>
        /// Callback gana set
        /// </summary>
        /// <param name="callback"></param>
        void IPartidoTenis.GanaSetCallback(Action<Set> callback)
        {
            ganaSetcallback += callback;
        }

        /// <summary>
        /// Callback gana Partido
        /// </summary>
        /// <param name="callback"></param>
        void IPartidoTenis.GanaPartidoCallback(Action<IPartidoTenis> callback)
        {
            newganaCallback += callback;
        }
        #endregion

        #endregion

        #endregion
    }


}
