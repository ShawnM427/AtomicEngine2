using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtomicEngine2.Utils
{
    /// <summary>
    /// Represents an object that get the corners for a texture atlas
    /// </summary>
    public class TextureManager
    {
        Texture2D _texture;
        int _texCount;
        byte _xTexs;
        byte _yTexs;

        /// <summary>
        /// What percentage each pixel represents
        /// </summary>
        float _xScale;
        /// <summary>
        /// What percentage each pixel represents
        /// </summary>
        float _yScale;

        /// <summary>
        /// Gets the texture
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
        }

        Vector2[,] _corners;

        /// <summary>
        /// Creates a new texture manager
        /// </summary>
        /// <param name="tex">The texture to bind to</param>
        /// <param name="xTexs">The number of textures in the x axis</param>
        /// <param name="yTexs">The number of textures in the y axis</param>
        public TextureManager(Texture2D tex, int xTexs, int yTexs)
        {
            _texture = tex;
            _texCount = xTexs * yTexs;

            _xTexs = (byte)xTexs;
            _yTexs = (byte)yTexs;

            _xScale = 1.0F / xTexs;
            _yScale = 1.0F / yTexs;

            BuildCorners();
        }

        /// <summary>
        /// Builds the corners for this texture
        /// </summary>
        private void BuildCorners()
        {
            _corners = new Vector2[_texCount, 4];

            for (int i = 0; i < _texCount; i++)
            {
                _corners[i, 0] =
                    new Vector2(GetXIndex(i) * _xScale, GetYIndex(i) * _yScale);
                _corners[i, 1] =
                    new Vector2((GetXIndex(i) + 1) * _xScale, GetYIndex(i) * _yScale);
                _corners[i, 2] =
                    new Vector2(GetXIndex(i) * _xScale, (GetYIndex(i) + 1) * _yScale);
                _corners[i, 3] =
                    new Vector2((GetXIndex(i) + 1) * _xScale, (GetYIndex(i) + 1) * _yScale);
            }
        }

        /// <summary>
        /// Gets the x index for the given texture ID
        /// </summary>
        /// <param name="texID">The texture ID to search for</param>
        /// <returns></returns>
        private int GetXIndex(int texID)
        {
            int y = (int)(texID / _yTexs);
            return texID - (int)(y * _xTexs);
        }

        /// <summary>
        /// Gets the y index for the given texture ID
        /// </summary>
        /// <param name="texID">The texture ID to search for</param>
        /// <returns></returns>
        private int GetYIndex(int texID)
        {
            return (int)(texID / _yTexs);
        }

        /// <summary>
        /// Gets the bottom-left texture co-ord for the texture ID
        /// </summary>
        /// <param name="ID">The texture ID to search for</param>
        /// <returns>The bottom-left corner for the texture refrence</returns>
        public Vector2 TL(int ID)
        {
            return _corners[ID, 0];
        }

        /// <summary>
        /// Gets the bottom-right texture co-ord for the texture ID
        /// </summary>
        /// <param name="ID">The texture ID to search for</param>
        /// <returns>The bottom-right corner for the texture refrence</returns>
        public Vector2 TR(int ID)
        {
            return _corners[ID, 1];
        }

        /// <summary>
        /// Gets the top-left texture co-ord for the texture ID
        /// </summary>
        /// <param name="ID">The texture ID to search for</param>
        /// <returns>The top-left corner for the texture refrence</returns>
        public Vector2 BL(int ID)
        {
            return _corners[ID, 2];
        }

        /// <summary>
        /// Gets the top-right texture co-ord for the texture ID
        /// </summary>
        /// <param name="ID">The texture ID to search for</param>
        /// <returns>The top-right corner for the texture refrence</returns>
        public Vector2 BR(int ID)
        {
            return _corners[ID, 3];
        }
    }
}
