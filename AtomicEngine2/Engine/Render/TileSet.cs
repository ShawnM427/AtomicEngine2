using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomicEngine2.Utils;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace AtomicEngine2.Engine.Render
{
    /// <summary>
    /// Represents a set of tiles
    /// </summary>
    public class TileSet
    {
        #region private Variables
        Texture2D _tex;

        int _texWidth;
        int _texHeight;

        int _tileWidth;
        int _tileHeight;

        byte _xTiles;
        byte _yTiles;

        string _name;

        Rectangle[] _sources;
        bool[] _solid;
        string[] _names;
        #endregion

        #region Public Properties
        /// <summary>
        /// The width of a tile in the texture
        /// </summary>
        public int TileHeight
        {
            get { return _tileHeight; }
        }
        /// <summary>
        /// The height of a tile in the texture
        /// </summary>
        public int TileWidth
        {
            get { return _tileWidth; }
        }

        /// <summary>
        /// Gets the name of this tile set
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        #endregion

        /// <summary>
        /// Creates a new tile set
        /// </summary>
        /// <param name="tex">The texture atlas to use</param>
        /// <param name="xTiles">The number of tiles in the texture along the x axis</param>
        /// <param name="yTiles">The number of tiles in the texture along the y axis</param>
        public TileSet(Texture2D tex, int xTiles, int yTiles, string name = "tileset")
        {
            if ((long)xTiles * (long)yTiles > 256)
                throw new InvalidOperationException("Tile count cannot be larger than that of a byte!");

            _tex = tex;

            _xTiles = (byte)xTiles;
            _yTiles = (byte)yTiles;
            
            _texHeight = tex.Height;
            _texWidth = tex.Width;

            _tileWidth = _texWidth / _xTiles;
            _tileHeight = _texHeight / _yTiles;

            _solid = new bool[_xTiles * _yTiles];
            _names = new string[_xTiles * _yTiles];

            _name = name;

            BuildSources();
        }

        /// <summary>
        /// Builds the source lookup list
        /// </summary>
        private void BuildSources()
        {
            _sources = new Rectangle[_xTiles * _yTiles];

            for (int y = 0; y < _yTiles; y++)
            {
                for (int x = 0; x < _xTiles; x++)
                {
                    _sources[y * _xTiles + x] =
                        new Rectangle(x * _tileWidth, y * _tileHeight, _tileWidth, _tileHeight);
                }
            }
        }

        #region Public Data Manipulation
        /// <summary>
        /// Sets the lookup name for a tile ID
        /// </summary>
        /// <param name="ID">The tile ID to set</param>
        /// <param name="name">The name for the tile</param>
        public void SetName(byte ID, string name)
        {
            name = name.ToLower();

            if (_names.Contains(name))
                throw new InvalidOperationException("A name cannot exist twice!");
            _names[ID] = name;
        }

        /// <summary>
        /// Gets the name for the specific tile ID
        /// </summary>
        /// <param name="ID">The tile ID to search for</param>
        /// <returns>The name for the given tile ID</returns>
        public string GetName(byte ID)
        {
            return _names[ID];
        }

        /// <summary>
        /// Gets the tile ID for a specific name
        /// </summary>
        /// <param name="name">The name to search for</param>
        /// <returns>The tile ID for the given name, or -1</returns>
        public byte GetID(string name)
        {
            name = name.ToLower();
            return (byte)Array.IndexOf(_names, name);
        }

        /// <summary>
        /// Sets whether or not the specified tile ID is solid
        /// </summary>
        /// <param name="ID">The tile ID to set</param>
        /// <param name="solid">Whether or not this tile is solid</param>
        public void SetSolid(byte ID, bool solid)
        {
            _solid[ID] = solid;
        }

        /// <summary>
        /// Gets whether a tile is solid
        /// </summary>
        /// <param name="ID">The tile ID to get</param>
        /// <returns>True if the tile type is solid</returns>
        public bool GetSolid(byte ID)
        {
            return _solid[ID];
        }

        /// <summary>
        /// Sets whether or not the specified tile is solid
        /// </summary>
        /// <param name="name">The name of the tile to set</param>
        /// <param name="solid">Whether or not this tile is solid</param>
        public void SetSolid(string name, bool solid)
        {
            _solid[GetID(name)] = solid;
        }

        /// <summary>
        /// Gets whether a tile is solid
        /// </summary>
        /// <param name="name">The name of the tile to get</param>
        /// <returns>True if the tile type is solid</returns>
        public bool GetSolid(string name)
        {
            return _solid[GetID(name)];
        }
        #endregion

        /// <summary>
        /// Draws a tile from this tile set
        /// </summary>
        /// <param name="batch">The sprite batch to use</param>
        /// <param name="destination">The destination of the tile</param>
        /// <param name="ID">The ID of the tile</param>
        public void Draw(SpriteBatch batch, Rectangle destination, byte ID)
        {
            batch.Draw(_tex, destination, _sources[ID], Color.White);
        }

        /// <summary>
        /// Writes this tile set to the stream
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void WriteToStream(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);

            _tex.SaveRaw(stream);
            writer.Write(_xTiles);
            writer.Write(_yTiles);
            writer.Write(_name);

            writer.Write(_names);
            writer.Write(_solid);
        }

        /// <summary>
        /// Reads a tileset from the stream
        /// </summary>
        /// <param name="graphics">The graphics device to bind the tile set to</param>
        /// <param name="stream">The stream to read from</param>
        /// <returns></returns>
        public static TileSet ReadFromStream(GraphicsDevice graphics, Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            Texture2D tex = new Texture2D(graphics, 0, 0);
            tex = tex.FromRaw(stream);
            int xTiles = reader.ReadByte();
            int yTiles = reader.ReadByte();
            string name = reader.ReadString();
            TileSet temp = new TileSet(tex, xTiles, yTiles, name);

            temp._names = reader.ReadStringArray();
            temp._solid = reader.ReadBooleanArray();

            return temp;
        }
    }
}
