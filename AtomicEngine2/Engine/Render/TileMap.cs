using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using AtomicEngine2.Utils;
using Microsoft.Xna.Framework;
using System.IO;
using AtomicEngine2.Engine.Input;
using Microsoft.Xna.Framework.Input;
using AtomicEngine2.Engine.Entities;

namespace AtomicEngine2.Engine.Render
{
    /// <summary>
    /// Represents a map that uses a texture atlas to render
    /// </summary>
    public class TileMap : GameComponent
    {
        #region Variables
        GraphicsDevice _graphics;
        SpriteBatch _spriteBatch;

        TileSet _tileSet;
        int _width;
        int _height;
        
        int _xTiles;
        int _yTiles;

        int _tileWidth;
        int _tileHeight;

        Rectangle[] _rects;
        byte[] _data;

        string _visibleName;

        Camera2D _camera;
        TDPlayer_8 _player;

        Texture2D _temp;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the width of this level in world coordinates
        /// </summary>
        public int Width
        {
            get { return _width; }
        }
        /// <summary>
        /// Gets the height of this level in  world coordinates
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Gets the width of this level's tiles
        /// </summary>
        public int TileWidth
        {
            get { return _tileWidth; }
        }
        /// <summary>
        /// Gets the height of this level's tiles
        /// </summary>
        public int TileHeight
        {
            get { return _tileHeight; }
        }

        /// <summary>
        /// Gets the number of tiles along the x axis
        /// </summary>
        public int XTiles
        {
            get { return _xTiles; }
        }
        /// <summary>
        /// Gets the number of tiles along the y axis
        /// </summary>
        public int YTiles
        {
            get { return _yTiles; }
        }

        /// <summary>
        /// Gets the visible name for this level
        /// </summary>
        public string VisibleName
        {
            get { return _visibleName; }
        }
        #endregion

        /// <summary>
        /// Creates a new tile map
        /// </summary>
        /// <param name="tileSet">The tile set to use</param>
        /// <param name="xTiles">The number of tiles along the x axis</param>
        /// <param name="yTiles">The number of tiles along the y axis</param>
        /// <param name="tileWidth">The width of each tile (optional)</param>
        /// <param name="tileHeight">The height of each tile (optional)</param>
        public TileMap(Game game, TileSet tileSet, int xTiles, int yTiles, 
            int? tileWidth = null, int? tileHeight = null, string visibleName = "level") : base(game)
        {
            _graphics = game.GraphicsDevice;
            _spriteBatch = new SpriteBatch(_graphics);

            _tileSet = tileSet;

            _xTiles = xTiles;
            _yTiles = yTiles;

            _tileHeight = tileHeight.HasValue ? tileHeight.Value : _tileSet.TileHeight;
            _tileWidth = tileWidth.HasValue ? tileWidth.Value : _tileSet.TileWidth;

            _width = _xTiles * _tileWidth;
            _height = _yTiles * _tileHeight;

            BuildRects();

            _data = new byte[_xTiles * _yTiles];
            _visibleName = visibleName;

            _player = new TDPlayer_8(new Vector2(100, 100), game.Content.Load<Texture2D>("TDPlayer"));

            _camera = new Camera2D(game);
            _camera.Focus = _player;
            _camera.TweenTrack = true;

            _temp = game.Content.Load<Texture2D>("atlas");

            game.Components.Add(this);
        }

        /// <summary>
        /// Builds the destination rectangles
        /// </summary>
        private void BuildRects()
        {
            _rects = new Rectangle[_xTiles * _yTiles];

            for (int y = 0; y < _yTiles; y++)
            {
                for (int x = 0; x < _xTiles; x++)
                {
                    _rects[y * _yTiles + x] = 
                        new Rectangle(x * _tileWidth, y * _tileHeight, _tileWidth, _tileHeight);
                }
            }
        }

        /// <summary>
        /// Sets the tile ID at the given x and y
        /// </summary>
        /// <param name="x">The x co-ord (in tile coordinates)</param>
        /// <param name="y">The y co-ord (in tile coordinates)</param>
        /// <param name="ID">The ID of the tile to set</param>
        public void SetData(int x, int y, byte ID)
        {
            _data[y * _xTiles + x] = ID;
        }

        /// <summary>
        /// Gets the tile ID at the given x and y
        /// </summary>
        /// <param name="x">The x co-ord (in tile coordinates)</param>
        /// <param name="y">The y co-ord (in tile coordinates)</param>
        /// <returns>The ID of the tile at (x,y)</returns>
        public byte GetData(int x, int y)
        {
            return _data[y * _xTiles + x];
        }

        /// <summary>
        /// Checks if a rectangle intersects a solid tile
        /// </summary>
        /// <param name="rect">The rectangle to check</param>
        /// <returns>True if the rectangle intersects a solid tile</returns>
        public bool Intersects(Rectangle rect)
        {
            //find min and max x and y
            // + 1 on max is to shift rounded off value  up by one
            int minTileX = (int)(rect.Left / _tileWidth); 
            int maxTileX = (int)(rect.Right / _tileWidth) + 1;
            int minTileY = (int)(rect.Top / _tileWidth);
            int maxTileY = (int)(rect.Bottom / _tileWidth) + 1;

            for (int x = minTileX; x < maxTileX; x++)
            {
                for (int y = minTileY; y < maxTileY; y++)
                {
                    if (rect.Intersects(_rects[y * _xTiles + x]) & _tileSet.GetSolid(_data[y * _xTiles + x]))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates this tile map
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);

            base.Update(gameTime);
        }
        
        /// <summary>
        /// Draws this tile map
        /// </summary>
        /// <param name="batch">Th sprite batch to use</param>
        public void Draw (GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null
            ,null, _camera.Transform);

            DrawLevel();
            _player.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();
        }

        /// <summary>
        /// Draws the tiles in this map
        /// </summary>
        protected void DrawLevel()
        {
            int rID;

            int minX = (int)(_camera.Left / _tileWidth);
            int minY = (int)(_camera.Top / _tileWidth);
            int maxX = (int)(_camera.Right / _tileWidth) + 1;
            int maxY = (int)(_camera.Bottom / _tileWidth) + 1;

            maxX = maxX > _xTiles ? _xTiles : maxX;
            maxY = maxY > _yTiles ? _yTiles : maxY;

            for (int y = minY < 0 ? 0 : minY; y < maxY; y++)
            {
                for (int x = minX < 0 ? 0 : minX; x < maxX; x++)
                {
                    rID = y * _xTiles + x;
                    _tileSet.Draw(_spriteBatch, _rects[rID], _data[rID]);
                }
            }
        }

        /// <summary>
        /// Writes this tile map to the stream
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void WriteToStream(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            
            _tileSet.WriteToStream(stream);

            writer.Write(_xTiles);
            writer.Write(_yTiles);
            writer.Write(_tileWidth);
            writer.Write(_tileHeight);

            writer.Write(_data);

            writer.Write(_visibleName);
        }

        /// <summary>
        /// Reads a tile map from the stream
        /// </summary>
        /// <param name="graphics">The graphics device to bind the map's tile set to</param>
        /// <param name="stream">The stream to read from</param>
        /// <returns></returns>
        public static TileMap ReadFromStream(Game game, Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            TileSet tileSet = TileSet.ReadFromStream(game.GraphicsDevice, stream);

            int xTiles = reader.ReadInt32();
            int yTiles = reader.ReadInt32();
            int tileWidth = reader.ReadInt32();
            int tileHeight = reader.ReadInt32();

            byte[] data = reader.ReadBytes(xTiles * yTiles);

            string name = reader.ReadString();

            TileMap temp = new TileMap(game, tileSet, xTiles, yTiles, tileWidth, tileHeight, name);
            temp._data = data;

            return temp;
        }
    }
}
