using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenisMatch.Class
{
    /// <summary>
    /// Representa al jugador del partido
    /// </summary>
    public class Jugador
    {
        #region Cosntructor
        public Jugador(string name)
        {
            Nombre = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Nombre del jugador
        /// </summary>
        public string Nombre { get; set; }


        /// <summary>
        /// Identificador Player1 - Player2
        /// </summary>
        public JugadorEnum Id { get;internal set ;}
        #endregion
    }


    /// <summary>
    /// Clase contenedora de lista de Jugadores
    /// </summary>
    public class Jugadores : List<Jugador>
    {
    
    }
}
