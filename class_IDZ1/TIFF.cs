using System;
using System.IO;
using System.Windows.Forms;

namespace IDZ1_library
{

    /// <summary>
    /// Класс для работы с файлами формата TIFF
    /// </summary>
    public class TIFF : FileDZZ
    {
        /// <summary>
        /// Структура тега заголовка
        /// </summary>
        public struct Tag
        {
            /// <summary>
            /// Код тега
            /// </summary>
            public ushort tag;
            /// <summary>
            /// Имя/название тега
            /// </summary>
            public string name;
            /// <summary>
            /// Тип данных тэга
            /// </summary>
            public ushort type;
            /// <summary>
            /// Название типа данных тэга
            /// </summary>
            public string type_data;
            /// <summary>
            /// Количество записей в теге
            /// </summary>
            public uint count_field;
            /// <summary>
            /// Данные тэга(в соответствии с форматом данных и количестве записей!)
            /// </summary>
            public object[] data;
            /// <summary>
            /// Указатель на данные
            /// </summary>
            public long pointer;
        }
        /// <summary>
        /// Количество тегов в заголовке
        /// </summary>
        public ushort Num_Tags;
        private uint Width;
        private uint Height;
        private uint SamplesPerPixel;
        /// <summary>
        /// Теги изображения
        /// </summary>
        public Tag[] Tags;
        /// <summary>
        /// Путь к файлу изображения
        /// </summary>
        public string path;
        /// <summary>
        /// Заголовок файла
        /// </summary>
        public ulong header;
        /// <summary>
        /// Версия файла. Значение должно быть строго равно 42!
        /// </summary>
        public ushort version;
        /// <summary>
        /// true for Intel ByteOrder(little-endian), false for Motorola ByteOrder(Big-Endian)
        /// </summary>
        internal bool byte_order;
        /// <summary>
        /// Ширина изображения
        /// </summary>
        public uint width
        {
            set
            {
                Width = value;
            }
            get
            {
                return Width;
            }
        }
        /// <summary>
        /// Высота изображения спектра
        /// </summary>
        public uint height
        {
            set
            {
                SamplesPerPixel = value;
            }
            get
            {
                return SamplesPerPixel;
            }
        }
        /// <summary>
        /// Количество кадров в файле
        /// </summary>
        public uint kadri
        {
            set
            {
                Height = value;
            }
            get
            {
                return Height;
            }
        }
        /// <summary>
        /// Количество байт на спектральный кадр
        /// </summary>
        public uint bytes_per_frame
        {
            get
            {
                return Width * SamplesPerPixel;
            }
        }


        /// <summary>
        /// Конструктор для нового объекта
        /// </summary>
        /// <param name="ptf">Путь к файлу, можно пропустить</param>
        public TIFF(string ptf = "")
        {
            if (ptf != "") LoadFile(ptf);
        }
        /// <summary>
        /// Загрузка заголовочной информации файла
        /// </summary>
        /// <param name="pathtofile">Путь к файлу</param>
        public void LoadFile(string pathtofile)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(pathtofile, FileMode.Open)))
            {
                byte_order = String.Concat<Char>(reader.ReadChars(2)) == "II" ? true : false;
                version = reader.ReadUInt16();
                if (version != 42 && version != 10752)
                {
                    MessageBox.Show("Ответ на главный вопрос жизни, вселенной и всего такого не был получен! Может файл испорчен?", "Ой какая незадача...",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                if (byte_order == true)
                {
                    reader.BaseStream.Position = reader.ReadUInt32();
                    Num_Tags = reader.ReadUInt16();
                    Tags = new Tag[Num_Tags];
                    for (int i = 0; i < Tags.Length; i++)
                    {
                        Tags[i].tag = reader.ReadUInt16();
                        Tags[i].name = GetTagName(Tags[i].tag);
                        Tags[i].type = reader.ReadUInt16();
                        Tags[i].count_field = reader.ReadUInt32();
                        if (Tags[i].count_field == 1 && Tags[i].type < 12 && Tags[i].type != 5 && Tags[i].type != 7 && Tags[i].type != 10)
                        {
                            Tags[i].data = new object[1];
                            #region загрузка значений, влезающие в 4 байта
                            switch (Tags[i].type)
                            {
                                case 1:
                                    Tags[i].type_data = "UBYTE";
                                    Tags[i].data[0] = (byte)reader.ReadUInt32();
                                    break;
                                case 2:
                                    Tags[i].type_data = "ASCII";
                                    Tags[i].data[0] = (char)(byte)reader.ReadUInt32();
                                    break;
                                case 3:
                                    Tags[i].type_data = "USHORT";
                                    Tags[i].data[0] = (ushort)reader.ReadUInt32();
                                    break;
                                case 4:
                                    Tags[i].type_data = "ULONG(UINT)";
                                    Tags[i].data[0] = reader.ReadUInt32();
                                    break;
                                case 6:
                                    Tags[i].type_data = "SBYTE";
                                    Tags[i].data[0] = (byte)reader.ReadUInt32();
                                    break;
                                case 8:
                                    Tags[i].type_data = "SSHORT";
                                    Tags[i].data[0] = (short)reader.ReadUInt32();
                                    break;
                                case 9:
                                    Tags[i].type_data = "SLONG(INT)";
                                    Tags[i].data[0] = reader.ReadInt32();
                                    break;
                                case 11:
                                    Tags[i].type_data = "FLOAT";
                                    Tags[i].data[0] = (float)reader.ReadSingle();
                                    break;
                            }
                            #endregion
                        } //if <=4 byte data
                        else
                        {
                            long pos = reader.BaseStream.Position + 4;
                            reader.BaseStream.Position = reader.ReadUInt32();
                            Tags[i].data = new object[Tags[i].count_field];
                            for (int k = 0; k < Tags[i].count_field; k++)
                            {
                                #region switch
                                switch (Tags[i].type)
                                {
                                    case 1:
                                        Tags[i].type_data = "UBYTE";
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        Tags[i].data[k] = reader.ReadByte();
                                        break;
                                    case 2:
                                        Tags[i].type_data = "CHAR";
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        Tags[i].data[k] = reader.ReadChar();
                                        break;
                                    case 3:
                                        Tags[i].type_data = "USHORT";
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        Tags[i].data[k] = reader.ReadUInt16();
                                        break;
                                    case 4:
                                        Tags[i].type_data = "ULONG(UINT32)";
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        Tags[i].data[k] = reader.ReadUInt32();
                                        break;
                                    case 5:
                                        Tags[i].type_data = "URATIONAL";
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        Tags[i].data[k] = reader.ReadUInt32().ToString() + "/" + reader.ReadUInt32().ToString();
                                        break;
                                    case 6:
                                        Tags[i].type_data = "SBYTE";
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        Tags[i].data[k] = reader.ReadSByte();
                                        break;
                                    case 7:
                                        Tags[i].type_data = "UNDEFINED(7)";
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        break;
                                    case 8:
                                        Tags[i].type_data = "SSHORT";
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        Tags[i].data[k] = reader.ReadInt16();
                                        break;
                                    case 9:
                                        Tags[i].type_data = "SLONG(INT32)";
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        Tags[i].data[k] = reader.ReadInt32();
                                        break;
                                    case 10:
                                        Tags[i].type_data = "SRATIONAL";
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        Tags[i].data[k] = reader.ReadInt32().ToString() + "/" + reader.ReadInt32().ToString();
                                        break;
                                    case 11:
                                        Tags[i].type_data = "FLOAT";
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        Tags[i].data[k] = reader.ReadSingle();
                                        break;
                                    case 12:
                                        Tags[i].type_data = "DOUBLE";
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        Tags[i].data[k] = reader.ReadDouble();
                                        break;
                                    default:
                                        Tags[i].pointer = reader.BaseStream.Position;
                                        Tags[i].type_data = "UNDEFINED";
                                        break;
                                }//switch 
                                #endregion
                            }//for k
                            reader.BaseStream.Position = pos;
                        } //else >4 byte data
                    } //for tags
                }//if little endian
                else
                {
                    #region big endian
                    byte[] a = reader.ReadBytes(4);
                    Array.Reverse(a);
                    reader.BaseStream.Position = BitConverter.ToInt64(a, 0);
                    a = reader.ReadBytes(2);
                    Array.Reverse(a);
                    Num_Tags = BitConverter.ToUInt16(a, 0);
                    Tags = new Tag[Num_Tags];
                    for (int i = 0; i < Tags.Length; i++)
                    {
                        a = reader.ReadBytes(2);
                        Array.Reverse(a);
                        Tags[i].tag = BitConverter.ToUInt16(a, 0);
                        Tags[i].name = GetTagName(Tags[i].tag);
                        a = reader.ReadBytes(2);
                        Array.Reverse(a);
                        Tags[i].type = BitConverter.ToUInt16(a, 0);
                        a = reader.ReadBytes(4);
                        Array.Reverse(a);
                        Tags[i].count_field = BitConverter.ToUInt32(a, 0);
                        if (Tags[i].count_field == 1 && Tags[i].type < 12 && Tags[i].type != 5 && Tags[i].type != 7 && Tags[i].type != 10)
                        {
                            Tags[i].data = new object[1];
                            #region загрузка значений, влезающие в 4 байта
                            switch (Tags[i].type)
                            {
                                case 1:
                                    Tags[i].type_data = "UBYTE";
                                    a = reader.ReadBytes(4);
                                    Array.Reverse(a);
                                    Tags[i].data[0] = (byte)BitConverter.ToUInt32(a, 0);
                                    break;
                                case 2:
                                    Tags[i].type_data = "ASCII";
                                    a = reader.ReadBytes(4);
                                    Array.Reverse(a);
                                    Tags[i].data[0] = (char)(byte)BitConverter.ToUInt32(a, 0);
                                    break;
                                case 3:
                                    Tags[i].type_data = "USHORT";
                                    a = reader.ReadBytes(4);
                                    Array.Reverse(a);
                                    Tags[i].data[0] = (ushort)BitConverter.ToUInt32(a, 0);
                                    break;
                                case 4:
                                    Tags[i].type_data = "ULONG(UINT)";
                                    a = reader.ReadBytes(4);
                                    Array.Reverse(a);
                                    Tags[i].data[0] = BitConverter.ToUInt32(a, 0);
                                    break;
                                case 6:
                                    Tags[i].type_data = "SBYTE";
                                    a = reader.ReadBytes(4);
                                    Array.Reverse(a);
                                    Tags[i].data[0] = (sbyte)reader.ReadUInt32();
                                    break;
                                case 8:
                                    Tags[i].type_data = "SSHORT";
                                    a = reader.ReadBytes(4);
                                    Array.Reverse(a);
                                    Tags[i].data[0] = (short)BitConverter.ToUInt32(a, 0);
                                    break;
                                case 9:
                                    Tags[i].type_data = "SLONG(INT)";
                                    a = reader.ReadBytes(4);
                                    Array.Reverse(a);
                                    Tags[i].data[0] = BitConverter.ToInt32(a, 0);
                                    break;
                                case 11:
                                    Tags[i].type_data = "FLOAT";
                                    a = reader.ReadBytes(4);
                                    Array.Reverse(a);
                                    Tags[i].data[0] = (float)BitConverter.ToSingle(a, 0);
                                    break;
                            }
                            #endregion
                        } //if <=4 byte data
                        else
                        {
                            long pos = reader.BaseStream.Position + 4;
                            a = reader.ReadBytes(4);
                            Array.Reverse(a);
                            reader.BaseStream.Position = BitConverter.ToUInt32(a, 0);
                            Tags[i].data = new object[Tags[i].count_field];
                            for (int k = 0; k < Tags[i].count_field; k++)
                            {
                                #region switch
                                switch (Tags[i].type)
                                {
                                    case 1:
                                        Tags[i].type_data = "UBYTE";
                                        Tags[i].data[k] = reader.ReadByte();
                                        break;
                                    case 2:
                                        Tags[i].type_data = "CHAR";
                                        Tags[i].data[k] = reader.ReadChar();
                                        break;
                                    case 3:
                                        Tags[i].type_data = "USHORT";
                                        a = reader.ReadBytes(2);
                                        Array.Reverse(a);
                                        Tags[i].data[k] = BitConverter.ToUInt16(a, 0);
                                        break;
                                    case 4:
                                        Tags[i].type_data = "ULONG(UINT32)";
                                        a = reader.ReadBytes(4);
                                        Array.Reverse(a);
                                        Tags[i].data[k] = BitConverter.ToUInt32(a, 0);
                                        break;
                                    case 5:
                                        Tags[i].type_data = "URATIONAL";
                                        a = reader.ReadBytes(4);
                                        Array.Reverse(a);
                                        Tags[i].data[k] = BitConverter.ToUInt32(a, 0) + "/";
                                        a = reader.ReadBytes(4);
                                        Array.Reverse(a);
                                        Tags[i].data[k] += BitConverter.ToUInt32(a, 0).ToString();
                                        break;
                                    case 6:
                                        Tags[i].type_data = "SBYTE";
                                        Tags[i].data[k] = reader.ReadSByte();
                                        break;
                                    case 7:
                                        Tags[i].type_data = "UNDEFINED(7)";
                                        break;
                                    case 8:
                                        Tags[i].type_data = "SSHORT";
                                        a = reader.ReadBytes(2);
                                        Array.Reverse(a);
                                        Tags[i].data[k] = BitConverter.ToInt16(a, 0);
                                        break;
                                    case 9:
                                        Tags[i].type_data = "SLONG(INT32)";
                                        a = reader.ReadBytes(4);
                                        Array.Reverse(a);
                                        Tags[i].data[k] = BitConverter.ToInt32(a, 0);
                                        break;
                                    case 10:
                                        Tags[i].type_data = "SRATIONAL";
                                        a = reader.ReadBytes(4);
                                        Array.Reverse(a);
                                        Tags[i].data[k] = BitConverter.ToInt32(a, 0) + "/";
                                        a = reader.ReadBytes(4);
                                        Array.Reverse(a);
                                        Tags[i].data[k] += BitConverter.ToInt32(a, 0).ToString();
                                        break;
                                    case 11:
                                        Tags[i].type_data = "FLOAT";
                                        a = reader.ReadBytes(4);
                                        Array.Reverse(a);
                                        Tags[i].data[k] = BitConverter.ToSingle(a, 0);
                                        break;
                                    case 12:
                                        Tags[i].type_data = "DOUBLE";
                                        a = reader.ReadBytes(8);
                                        Array.Reverse(a);
                                        Tags[i].data[k] = BitConverter.ToDouble(a, 0);
                                        break;
                                    default:
                                        Tags[i].type_data = "UNDEFINED";
                                        break;
                                }//switch 
                                #endregion
                            }//for k
                            reader.BaseStream.Position = pos;
                        } //else >4 byte data
                    } //for tags 
                    #endregion
                }//else big endian
                if (reader.ReadUInt32() != 0)
                {
                    MessageBox.Show("В файле находятся несколько заголовков изображений! Работа будет производиться с первым изображением.",
                        "Несколько изображений", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                try
                {
                    this.width = (uint)this.Tags[GetIndexByTag(256)].data[0];
                    this.height = (uint)this.Tags[GetIndexByTag(257)].data[0];
                    this.kadri = (uint)this.Tags[GetIndexByTag(277)].data[0];
                    object o = this.Tags[GetIndexByTag(273)];
                    o = this.Tags[GetIndexByTag(284)];
                }
                catch (IndexOutOfRangeException)
                {
                    MessageBox.Show("В заголовке изображения не обнаружены обязательные теги, файл поврежден!",
                        "Файл поврежден", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    throw;
                }
            } //Using binaryreader
        }//LoadFile
        /// <summary>
        /// Возвращает спектральный кадр по заданным границам, пикселы вне границ становятся нулями
        /// </summary>
        /// <param name="k">Номер кадра [1..n]</param>
        /// <param name="lg">Количество пиксел для пропуска слева</param>
        /// <param name="pg">Количество пиксел для пропуска справа</param>
        /// <returns>Кадр</returns>
        public int[,] LoadFrameSpectral(int k, int lg=0, int pg=0)
        {
            int[,] frame = new int[this.width, this.height];
            bool planar = false;
            if ((ushort)Tags[GetIndexByTag(284)].data[0] == 1) planar = true; //проверка тега на структуру записи изображения
            int off = GetIndexByTag(273);
            if (k < 1 || k > kadri) throw new ArgumentException("Значение кадра должно быть больше нуля и не больше количества кадров в файле!");
            if (planar == true)
            {
                if (Tags[off].count_field != Height * SamplesPerPixel) throw new SystemException("Количество записей смещений до строк изображения не совпадает с размерами изображения.");
                using (BinaryReader reader = new BinaryReader(File.Open(this.path, FileMode.Open)))
                {
                    for (int i = 0; i < height; i++)
                    {
                        reader.BaseStream.Position = (long)Tags[off].data[i * kadri + k - 1];
                        for (int j = lg; j < width - pg; j++) frame[j, i] = reader.ReadInt16();
                    }
                } // reader
            }//if planar == true
            else
            {
                if (Tags[off].count_field != kadri) throw new SystemException("Количество записей смещений до строк изображения не совпадает с размерами изображения.");
                using (BinaryReader reader = new BinaryReader(File.Open(this.path, FileMode.Open)))
                {
                    reader.BaseStream.Position = (long)Tags[off].data[(k - 1)];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = lg; j < width - pg; j++) frame[j, i] = reader.ReadInt16();
                    }
                } // reader
            }//if planar == false
            if (byte_order == false)
            {
                for (int i = 0; i < height; i++) for (int j = 0; j < width; j++) frame[j, i] = frame[j, i] >> 8 | ((frame[j, i] & 0xFF) << 8);
            }
            if (lg > 0)
            {
                for (int i = 0; i < height; i++) for (int j = 0; j < lg; j++) frame[j, i] = 0;
            }
            if (pg > 0)
            {
                for (int i = 0; i < height; i++) for (int j = (int)(width - pg); j < width; j++) frame[j, i] = 0;
            }
            return frame;
        }
        /// <summary>
        /// Возвращает указанный кадр фильма(изображение)
        /// </summary>
        /// <param name="k">Номер кадра [1..n]</param>
        /// <param name="lg">Левая граница в кадре</param>
        /// <param name="pg">Правая граница в кадре</param>
        /// <returns>Кадр фильма(изображение)</returns>
        public int[,] LoadFrameFilm(int k, int lg = 0, int pg = 0)
        {
            int[,] frame = new int[this.width, this.kadri];
            bool planar = false;
            if ((ushort)Tags[GetIndexByTag(284)].data[0] == 1) planar = true; //проверка тега на структуру записи изображения
            int off = GetIndexByTag(273);
            if (k < 1 || k > height) throw new ArgumentException("Значение кадра должно быть больше нуля и не больше высоты спектра в файле!");
            if (planar == true)
            {
                if (Tags[off].count_field != Height * SamplesPerPixel) throw new SystemException("Количество записей смещений до строк изображения не совпадает с размерами изображения.");
                using (BinaryReader reader = new BinaryReader(File.Open(this.path, FileMode.Open)))
                {
                    for (int i = 0; i < kadri; i++)
                    {
                        reader.BaseStream.Position = (long)Tags[off].data[kadri * (k - 1) + i] + lg * 2;
                        for (int j = lg; j < width - pg; j++) frame[j, i] = reader.ReadInt16();
                    }
                } // reader
            }//if planar == true
            else
            {
                if (Tags[off].count_field != kadri) throw new SystemException("Количество записей смещений до строк изображения не совпадает с размерами изображения.");
                using (BinaryReader reader = new BinaryReader(File.Open(this.path, FileMode.Open)))
                {
                    reader.BaseStream.Position = (long)Tags[off].data[k - 1];
                    for (int i = 0; i < kadri; i++)
                    {
                        for (int j = lg; j < width - pg; j++) frame[j, i] = reader.ReadInt16();
                    }
                } // reader
            }//if planar == false
            if (byte_order == false)
            {
                for (int i = 0; i < kadri; i++) for (int j = 0; j < width; j++) frame[j, i] = frame[j, i] >> 8 | ((frame[j, i] & 0xFF) << 8);
            }
            if (lg > 0)
            {
                for (int i = 0; i < kadri; i++) for (int j = 0; j < lg; j++) frame[j, i] = 0;
            }
            if (pg > 0)
            {
                for (int i = 0; i < kadri; i++) for (int j = (int)(width - pg); j < width; j++) frame[j, i] = 0;
            }
            return frame;
        }
        /// <summary>
        /// Возвращает индекс указанного тэга в списке Tags
        /// </summary>
        /// <param name="x">Код тега</param>
        /// <returns>Индекс тега</returns>
        private int GetIndexByTag(int x) 
        {
            for (int i = 0; i < Num_Tags; i++) if (Tags[i].tag == x) return i;
            return -1;
        }//GetIndexByTag
        private string GetTagName(ushort tag_num)
        {
            switch (tag_num)
            {
                case 0: return "GPSVersionID";
                case 1: return "GPSLatitudeRef";
                case 2: return "GPSLatitude";
                case 3: return "GPSLingitudeRef";
                case 4: return "GPSLongitude";
                case 5: return "GPSAltitudeRef";
                case 6: return "GPSAltitude";
                case 7: return "GPSTimeStamp";
                case 8: return "GPSSatellites";
                case 9: return "GPSStatus";
                case 10: return "GPSMeasureMode";
                case 11: return "GPSDOP";
                case 12: return "GPSSpeedRef";
                case 13: return "GPSSpeed";
                case 14: return "GPSTrackRef";
                case 15: return "GPSTrack";
                case 16: return "GPSImgDirectionRef";
                case 17: return "GPSImgDirection";
                case 18: return "GPSMapDatum";
                case 19: return "GPSDesLatitudeRef";
                case 20: return "GPSDesLatitude";
                case 21: return "GPSDestLongitudeRef";
                case 22: return "GPSDestLongitude";
                case 23: return "GPSBearingRef";
                case 24: return "GPSBearing";
                case 25: return "GPSDistanceRef";
                case 26: return "GPSDistance";
                case 27: return "GPSProcessingMethod";
                case 28: return "GPSAreaInformation";
                case 29: return "GPSDateStamp";
                case 30: return "GPSDifferential";
                case 254: return "NewSubFileType";
                case 255: return "SubFileType";
                case 256: return "ImageWidth";
                case 257: return "ImageLength";
                case 258: return "BitsPerSample";
                case 259: return "Compression";
                case 262: return "PhotometricInterpretation";
                case 263: return "Thresholding";
                case 264: return "CellWidth";
                case 265: return "CellLength";
                case 266: return "FillOrder";
                case 269: return "DocumentName";
                case 270: return "ImageDescription";
                case 271: return "Make";
                case 272: return "Model";
                case 273: return "StripOffsets";
                case 274: return "Orientation";
                case 277: return "SamplesPerPixel";
                case 278: return "RowsPerStrip";
                case 279: return "StripByteCounts";
                case 280: return "MinSampleValue";
                case 281: return "MaxSampleValue";
                case 282: return "XResolution";
                case 283: return "YResolution";
                case 284: return "PlanarConfiguration";
                case 285: return "RageName";
                case 286: return "XPosition";
                case 287: return "YPosition";
                case 288: return "FreeOffsets";
                case 289: return "FreeByteCounts";
                case 290: return "GrayResponceUnit";
                case 291: return "GrayResponceCurve";
                case 292: return "T4Options";
                case 293: return "T6Options";
                case 296: return "ResolutionUnit";
                case 297: return "PageNumber";
                case 300: return "ColorResponceUnit";
                case 301: return "TransferFunction";
                case 305: return "Software";
                case 306: return "DateTime";
                case 315: return "Artist";
                case 316: return "HostComputer";
                case 317: return "Predictor";
                case 318: return "WhitePoint";
                case 319: return "PrimaryChromaticities";
                case 320: return "ColorMap";
                case 321: return "HalftoneHints";
                case 322: return "TileWidth";
                case 323: return "TileLength";
                case 324: return "TileOffsets";
                case 325: return "TileByteCounts";
                case 326: return "BadFaxLines";
                case 327: return "CleanFaxData";
                case 328: return "ConsecutiveBadFaxLines";
                case 330: return "SubIFDs";
                case 332: return "InkSet";
                case 333: return "InkNames";
                case 334: return "NumberOfInks";
                case 336: return "DotRange";
                case 337: return "TargetPrinter";
                case 338: return "ExtraSamples";
                case 339: return "SampleFormat";
                case 340: return "SMinSampleValue";
                case 341: return "SMaxSampleValue";
                case 342: return "TransferRange";
                case 343: return "ClipPath";
                case 344: return "XClipPathUnits";
                case 345: return "YClipPathUnits";
                case 346: return "Indexed";
                case 347: return "JPEGTables";
                case 351: return "OPIProxy";
                case 400: return "GlobalParametersIFD";
                case 401: return "ProfileType";
                case 402: return "FaxProfile";
                case 403: return "CodingMethods";
                case 404: return "VersionYear";
                case 405: return "ModeNumber";
                case 433: return "Decode";
                case 434: return "DefaultImageColor";
                case 512: return "JPEGProc";
                case 513: return "JPEGInterchangeFormat";
                case 514: return "JPEGInterchangeFormatLength";
                case 515: return "JPEGRestartInterval";
                case 517: return "JPEGLosslessPredictors";
                case 518: return "JPEGPointTransforms";
                case 519: return "JPEGQTables";
                case 520: return "JPEGDCTables";
                case 521: return "JPEGACTables";
                case 529: return "YCbCrCoefficients";
                case 530: return "YCbCrSubSampling";
                case 531: return "YCbCrPositioning";
                case 532: return "ReferenceBlackWhite";
                case 559: return "StripRowCounts";
                case 700: return "XMP";
                case 32781: return "ImageID";
                case 32932: return "Wang Annotation";
                case 33432: return "Copyright";
                case 33434: return "ExposureTime";
                case 33437: return "FNumber";
                case 33445: return "MD FileTag";
                case 33446: return "MD ScalePixel";
                case 33447: return "MD ColorTable";
                case 33448: return "MD LabName";
                case 33449: return "MD SampleInfo";
                case 33450: return "MD PrepDate";
                case 33451: return "MD PrepTime";
                case 33452: return "MD FileUnits";
                case 33550: return "ModelPixelScaleTag";
                case 33723: return "IPTC";
                case 33918: return "INGR Packet Data Tag";
                case 33919: return "INGR Flag Registers";
                case 33920: return "IrasB Transformation Matrix";
                case 33922: return "ModelTiepointTag";
                case 34264: return "ModelTransformationTag";
                case 34377: return "Photoshop";
                case 34665: return "Exif IFD";
                case 34675: return "ICC Profile";
                case 34732: return "ImageLayer";
                case 34735: return "GeoKeyDirectoryTag";
                case 34736: return "GeoDoubleParamsTag";
                case 34737: return "GeoAsciiParamsTag";
                case 34850: return "ExposureProgram";
                case 34852: return "SpectralSensitivity";
                case 34853: return "GPS IFD";
                case 34855: return "ISOSpeedRatings";
                case 34856: return "OECF";
                case 34908: return "HylaFAX FaxRecvParams";
                case 34909: return "HylaFAX FaxSubAddress";
                case 34910: return "HylaFAX FaxRecvTime";
                case 36864: return "ExifVersion";
                case 36867: return "DateTimeOriginal";
                case 36868: return "DateTimeDigitized";
                case 37121: return "ComponentsConfiguration";
                case 37122: return "CompressedBitsPerPixel";
                case 37377: return "ShutterSpeedValue";
                case 37378: return "ApertureValue";
                case 37379: return "BrightnessValue";
                case 37380: return "ExposureBiasValue";
                case 37381: return "MaxApertureValue";
                case 37382: return "SubjectDistance";
                case 37383: return "MeteringMode";
                case 37384: return "LightSource";
                case 37385: return "Flash";
                case 37386: return "FocalLength";
                case 37396: return "SubjectArea";
                case 37500: return "MakerNote";
                case 37510: return "UserComment";
                case 37520: return "SubsecTime";
                case 37521: return "SubsecTimeOriginal";
                case 37522: return "SubsecTimeDigitized";
                case 37724: return "ImageSourceData";
                case 40960: return "FlashpixVersion";
                case 40961: return "ColorSpace";
                case 40962: return "PixelXDimension";
                case 40963: return "PixelYDimension";
                case 40964: return "RelatedSoundFile";
                case 40965: return "Interoperability IFD";
                case 41483: return "FlashEnergy";
                case 41484: return "SpatialFrequencyResponse";
                case 41486: return "FocalPlaneXResolution";
                case 41487: return "FocalPlaneYResolution";
                case 41488: return "FocalPlaneResolutionUnit";
                case 41492: return "SubjectLocation";
                case 41493: return "ExposureIndex";
                case 41495: return "SensingMethod";
                case 41728: return "FileSource";
                case 41729: return "SceneType";
                case 41730: return "CFAPattern";
                case 41985: return "CustomRendered";
                case 41986: return "ExposureMode";
                case 41987: return "WhiteBalance";
                case 41988: return "DigitalZoomRatio";
                case 41989: return "FocalLengthIn35mmFilm";
                case 41990: return "SceneCaptureType";
                case 41991: return "GainControl";
                case 41992: return "Contrast";
                case 41993: return "Saturation";
                case 41994: return "Sharpness";
                case 41995: return "DeviceSettingDescription";
                case 41996: return "SubjectDistanceRange";
                case 42016: return "ImageUniqueID";
                case 42112: return "GDAL_METADATA";
                case 42113: return "GDAL_NODATA";
                case 50215: return "Oce Scanjob Description";
                case 50216: return "Oce Application Selector";
                case 50217: return "Oce Identification Number";
                case 50218: return "Oce ImageLogic Characteristics";
                case 50706: return "DNGVersion";
                case 50707: return "DNGBackwardVersion";
                case 50708: return "UniqueCameraModel";
                case 50709: return "LocalizedCameraModel";
                case 50710: return "CFAPlaneColor";
                case 50711: return "CFALayout";
                case 50712: return "LinearizationTable";
                case 50713: return "BlackLevelRepeatDim";
                case 50714: return "BlackLevel";
                case 50715: return "BlackLevelDeltaH";
                case 50716: return "BlackLevelDeltaV";
                case 50717: return "WhiteLevel";
                case 50718: return "DefaultScale";
                case 50719: return "DefaultCropOrigin";
                case 50720: return "DefaultCropSize";
                case 50721: return "ColorMatrix1";
                case 50722: return "ColorMatrix2";
                case 50723: return "CameraCalibration1";
                case 50724: return "CameraCalibration2";
                case 50725: return "ReductionMatrix1";
                case 50726: return "ReductionMatrix2";
                case 50727: return "AnalogBalance";
                case 50728: return "AsShotNeutral";
                case 50729: return "AsShotWhiteXY";
                case 50730: return "BaselineExposure";
                case 50731: return "BaselineNoise";
                case 50732: return "BaselineSharpness";
                case 50733: return "BayerGreenSplit";
                case 50734: return "LinearResponseLimit";
                case 50735: return "CameraSerialNumber";
                case 50736: return "LensInfo";
                case 50737: return "ChromaBlurRadius";
                case 50738: return "AntiAliasStrength";
                case 50740: return "DNGPrivateData";
                case 50741: return "MakerNoteSafety";
                case 50778: return "CalibrationIlluminant1";
                case 50779: return "CalibrationIlluminant2";
                case 50780: return "BestQualityScale";
                case 50784: return "Alias Layer Metadata";

                default: return "UNDEFINED";
            }
        }
        /// <summary>
        /// Вызывает форму отображения информации о файле
        /// </summary>
        public void ShowData()
        {
            INFO_TIF inf = new INFO_TIF(this);
            //inf.Show();
        }


    }

}
