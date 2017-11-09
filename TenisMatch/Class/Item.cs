using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenisMatch.Class
{


    /// <summary>
    /// Enumeracion de los diferentes estados que se encuentra un item
    /// Entedemos por Item (Partido-Set-Juego-Punto)
    /// </summary>
    public enum Estado
    {
        NoJugado,
        EnJuego,
        Terminado
    }

    /// <summary>
    /// Enumeracion de jugadores
    /// </summary>
    public enum JugadorEnum
    {
        None,
        Player1=1,
        Player2=2
    }
    

    /// <summary>
    /// Es la clase base donde heredaran las diferentes figuras que se compone un partido (Partido->Set->Juego->Puntos)
    /// </summary>
    /// <remarks>
    /// Es el 'corazon' del programa 
    /// </remarks>
    /// <typeparam name="T">Tipo base, utilizado para poder llamar a los callback de forma tipada</typeparam>
    public class ItemBase<T> where T : ItemBase<T>
    {
        #region var members
        protected Action<T> comienzoCallback; //callback comienzo
        protected Action<T> ganaCallback;///callback ganar
        #endregion

        #region Properties
        /// <summary>
        /// El estado en que se encuentra
        /// </summary>
        [DefaultValue(Estado.NoJugado)]
        public Estado Estado
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indica el Jugador que ha ganado
        /// </summary>
        public JugadorEnum JugadorGana   { get;  internal set; }


        /// <summary>
        /// Indica el Jugador que ha perdido
        /// </summary>
        public JugadorEnum JugadorPierde { get;internal set;}


        /// <summary>
        /// Puntos en valor numerico del jugador1
        /// </summary>
        public int PuntosJugador1 { get; internal set; }

        /// <summary>
        /// Puntos en valor numerico del jugador2
        /// </summary>
        public int PuntosJugador2 { get; internal set; }

        #endregion

        #region Methods
        /// <summary>
        /// Callback que se desencadena cuando comienza a jugarse ese item
        /// </summary>
        /// <param name="action"></param>
        public void ComienzoCallback(Action<T> action)
        {
            comienzoCallback += action;
        }

        /// <summary>
        /// Callback que se desencadena cuando un jugador gana ese item
        /// </summary>
        /// <param name="action"></param>
        public void GanaCallback(Action<T> action)
        {
            ganaCallback += action;
        }


        /// <summary>
        /// Metodo utilizado para el lanzamiento del callback de comienzo
        /// </summary>
        /// <param name="action"></param>
        protected virtual void OnComienzo(Action<T> action)
        {
            Estado = Estado.EnJuego;
            if (action != null)
                action((T)this);

            
        }

        /// <summary>
        /// Metodo utilizado para el lanzamiento del callback de ganar
        /// </summary>
        /// <param name="action"></param>
        protected virtual void OnGana(Action<T> action)
        {
            Estado = Estado.Terminado;
            if (action != null)
                action((T)this);
        }
        #endregion

    }



    /// <summary>
    /// Esta clase hereda de la itemBase y contiene elementos contenedores para otros subojetos
    /// </summary>
    /// <remarks>
    /// /// Por ej. Un Partido tiene Sets.
    ///         Un Set tiene Juegos
    /// Pero un punto no contine otros objetos por lo que no hereda de Item sino de ItemBase
    /// </remarks>
    /// <typeparam name="T">Tipo</typeparam>
    /// <typeparam name="I">Item contenido</typeparam>
    /// <typeparam name="C">Coleccion que contiene a los Items</typeparam>
    public class Item<T, I, C> : ItemBase<T> where T : ItemBase<T> where C : Items<I>, new() where I: ItemBase<I>,new()
    {
        #region Var Members
        private C _items;
        #endregion

        #region Constructor
        public Item() 
        {
            //Inicializamos contenedor
            _items = new C();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Item actualmente en Juego
        /// </summary>
        /// <remarks>
        /// Por ej. El Set que se encuentra en juego dentro de un partido , o el Punto dentro de un Juego
        /// </remarks>
        public I EnJuego
        {
            get; protected set;
        }

        /// <summary>
        /// Coleccion de items contenidos.
        /// </summary>
        protected Items<I> Items
        {
            get { return _items; }
        }
        #endregion


        /// <summary>
        /// Lanzado cuando se comienza un item
        /// </summary>
        /// <param name="item"></param>
        public virtual void Comenzar(I item)
        {
            //item.Estado = Estado.EnJuego;
            _items.Add(item);
            EnJuego = item;
            if (Estado == Estado.NoJugado)
            {
                Estado = Estado.EnJuego;
                OnComienzo(comienzoCallback);
            }
            
        }


        /// <summary>
        /// Invocado cuando un jugador gana un Item
        /// </summary>
        /// <param name="item"></param>
        public virtual void Ganar(I item)
        {
            if (item.Estado != Estado.EnJuego)
                throw new InvalidOperationException("No esta en juego");

            if (!_items.Contains(item))
                throw new InvalidOperationException("El item no se ha comenzado correctamente");

            
            item.Estado = Estado.Terminado;

            //Sumamos al contador de puntos del ganador del item

            if (HaGanado( item))
            {
                //Copiamos el ganador de ese Item(Punto/Juego/Set/Partido)
                JugadorGana = item.JugadorGana;
                JugadorPierde = item.JugadorPierde;

                OnGana(ganaCallback);
            }

            EnJuego = null;
        }

        /// <summary>
        /// Invocado para chequear si ha ganado 
        /// aqui debe establecerse la logica 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual bool HaGanado(I item) 
        {
            throw new NotImplementedException();
        }

      

    }

    /// <summary>
    /// Clase utilizada para contener objetos
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Items<T>: List<T> where T:ItemBase<T>
    {

        #region Methods
        /// <summary>
        /// Metodo para chequear si hay un objeto 'en juego'
        /// </summary>
        /// <returns>true si se esta jugando</returns>
        public bool ExistEnJuego()
        {
            return (FindEnJuego() == null);
        }

        /// <summary>
        /// Encuentra el objeto que esta en juego dentro de la coleccion,(Si lo hay)
        /// </summary>
        /// <returns>El objeto o NULL si no lo hay </returns>
        public T FindEnJuego()
        {
            return this.First(s => s.Estado == Estado.EnJuego);
            
        }
        #endregion
    }
}
