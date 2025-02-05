using System;
using System.IO;
using System.Text.Json;

namespace IisKnife
{
    /// <summary />
    public abstract class CommandBase
    {
        /// <summary />
        protected T Load<T>( string fileName )
        {
            if ( fileName == null )
                return Activator.CreateInstance<T>();


            /*
             * 
             */
            T obj;

            using ( var stream = new FileStream( fileName, FileMode.Open ) )
            {
                obj = JsonSerializer.Deserialize<T>( stream );
            }

            return obj;
        }
    }
}