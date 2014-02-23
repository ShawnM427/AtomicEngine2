using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework;

namespace AtomicEngine2.Utils
{
    public static class IOExtensions
    {
        /// <summary>
        /// Writes a string array to the stream
        /// </summary>
        /// <param name="writer">The binary writer to use</param>
        /// <param name="value">The array to write</param>
        public static void Write(this BinaryWriter writer, string[] value)
        {
            writer.Write(value.Length);

            for (int i = 0; i < value.Length; i++)
                writer.Write(value[i] == null ? "" : value[i]);
        }

        /// <summary>
        /// Writes a boolean array to the stream
        /// </summary>
        /// <param name="writer">The binary writer to use</param>
        /// <param name="value">The array to write</param>
        public static void Write(this BinaryWriter writer, bool[] value)
        {
            writer.Write(value.Length);

            for (int i = 0; i < value.Length; i++)
                writer.Write(value[i]);
        }

        /// <summary>
        /// Writes a Byte4 array to the stream
        /// </summary>
        /// <param name="writer">The binary writer to use</param>
        /// <param name="value">The array to write</param>
        public static void Write(this BinaryWriter writer, Byte4[] value)
        {
            writer.Write(value.Length);

            for (int i = 0; i < value.Length; i++)
                writer.Write(value[i]);
        }

        /// <summary>
        /// Writes a Byte4 to the stream
        /// </summary>
        /// <param name="writer">The binary writer to use</param>
        /// <param name="value">The value to write</param>
        public static void Write(this BinaryWriter writer, Byte4 value)
        {
            writer.Write(value.PackedValue);
        }

        /// <summary>
        /// Reads a string array from the stream
        /// </summary>
        /// <param name="reader">The reader to use</param>
        /// <returns>The string array read from the stream</returns>
        public static string[] ReadStringArray(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            string[] temp = new string[length];

            for (int i = 0; i < length; i++)
                temp [i] = reader.ReadString();

            return temp;
        }

        /// <summary>
        /// Reads a boolean array from the stream
        /// </summary>
        /// <param name="reader">The reader to use</param>
        /// <returns>The boolean array read from the stream</returns>
        public static bool[] ReadBooleanArray(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            bool[] temp = new bool[length];

            for (int i = 0; i < length; i++)
                temp[i] = reader.ReadBoolean();

            return temp;
        }

        /// <summary>
        /// Reads a Byte4 array from the stream
        /// </summary>
        /// <param name="reader">The reader to use</param>
        /// <returns>The Byte4 array read from the stream</returns>
        public static Byte4[] ReadByte4Array(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            Byte4[] temp = new Byte4[length];

            for (int i = 0; i < length; i++)
                temp[i] = reader.ReadByte4();

            return temp;
        }

        /// <summary>
        /// Reads a Byte4 to the stream
        /// </summary>
        /// <param name="reader">The binary reader to use</param>
        /// <returns>The value read from the stream</returns>
        public static Byte4 ReadByte4(this BinaryReader reader)
        {
            Byte4 temp = new Byte4();
            temp.PackedValue = reader.ReadUInt32();
            return temp;
        }

        public static void SaveRaw(this Texture2D _tex, Stream stream)
        {
            BinaryWriter w = new BinaryWriter(stream);
            Byte4[] data = new Byte4[_tex.Width * _tex.Height];
            _tex.GetData<Byte4>(data);

            w.Write(_tex.Width);
            w.Write(_tex.Height);
            w.Write(data);
        }

        /// <summary>
        /// Loads this image with raw image data pulled from the stream
        /// </summary>
        /// <param name="tex">The texture to use for loading</param>
        /// <param name="stream">The stream to load from</param>
        public static Texture2D FromRaw(this Texture2D tex, Stream stream)
        {
            BinaryReader r = new BinaryReader(stream);

            int width = r.ReadInt32();
            int height = r.ReadInt32();

            Byte4[] data = new Byte4[width * height];
            data = r.ReadByte4Array();

            tex = new Texture2D(tex.GraphicsDevice, width, height);
            tex.SetData<Byte4>(data);

            return tex;
        }
    }
}
