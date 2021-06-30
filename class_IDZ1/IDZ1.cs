using System;
using System.IO;
using System.Windows.Forms;

namespace IDZ1_library
{
    /// <summary>
    /// Класс для хранения информации из файлов IDZ и IDZ1
    /// </summary>
    public class IDZ1
    {
        #region
        /// <summary>
        /// Формат файла (idz, idz1, ima, imx)
        /// </summary>
        public string format;
        /// <summary>
        /// Ширина кадра
        /// </summary>
        public ushort width;//Количество элементов в строке
        /// <summary>
        /// Высота кадра
        /// </summary>
        public ushort height;//Количество строк
        /// <summary>
        /// Количество элементов
        /// </summary>
        public ushort N_el;
        /// <summary>
        /// Количество строк
        /// </summary>
        public byte N_st;
        /// <summary>
        /// Номера границ зон
        /// </summary>
        public byte[] kg = new byte[6]; //номера первых строк зон 1-6
        /// <summary>
        /// Коэффициенты бинирования
        /// </summary>
        public byte[] kbsb = new byte[6];
        /// <summary>
        /// КБСТ КДЧК
        /// </summary>
        public byte kbst, kdchk;
        /// <summary>
        /// Номер строк перескана
        /// </summary>
        public byte KS1, KS2;
        /// <summary>
        /// Признак включения ламп
        /// </summary>
        public bool LK1, LK2;
        /// <summary>
        /// Коэффициенты усиления 1
        /// </summary>
        public ushort[] amp1 = new ushort[4];
        /// <summary>
        /// Коэффициенты усиления 2
        /// </summary>
        public ushort[] amp2 = new ushort[4];
        /// <summary>
        /// Температура корпуса
        /// </summary>
        public short temp_k;
        /// <summary>
        /// Температура матрицы
        /// </summary>
        public short temp_m;
        /// <summary>
        /// Логические зоны
        /// </summary>
        public short[] logical_zone = new short[6];
        /// <summary>
        /// Количество кадров
        /// </summary>
        public uint kadri;
        /// <summary>
        /// Количество байт на кадр
        /// </summary>
        public uint bytes_per_frame;
        /// <summary>
        /// массив координат точек фрагментов [k]{x1,y1,x2,y2}
        /// </summary>
        public ushort[][] frags;
        /// <summary>
        /// Количество фрагментов
        /// </summary>
        public ushort kol_frags;
        /// <summary>
        /// Код визуализации
        /// </summary>
        public ushort kodeV;
        /// <summary>
        /// Путь к файлу idz
        /// </summary>
        private string path;
        #endregion

        /// <summary>
        /// Метод вычисляет границы логических зон для данного экземпляра класса
        /// </summary>
        public void log_zones()
        {
            logical_zone[0] = Convert.ToInt16( (this.kg[1] - 1 - this.kg[0]) / kbsb[0] + 1 );//верх логич граница 1-й зоны
            logical_zone[1] = Convert.ToInt16( (this.kg[2] - this.kg[1]) / kbsb[1] + logical_zone[0] );//верх логич граница 2-й зоны
            logical_zone[2] = Convert.ToInt16( (109 - this.kg[2]) / kbsb[2] + logical_zone[1] );//верх логич граница 3-й зоны
            logical_zone[3] = Convert.ToInt16( Math.Abs( 109 - this.kg[3] ) / kbsb[3] + logical_zone[2] );//верх логич граница 4-й зоны +2  
            logical_zone[4] = Convert.ToInt16( Math.Abs( (this.kg[3] - this.kg[4]) / kbsb[4] + logical_zone[3] ) );//верх логич граница 5-й зоны+2
            logical_zone[5] = Convert.ToInt16( Math.Abs( (this.kg[4] - this.kg[5]) / kbsb[5] + logical_zone[4] ) );//верх логич граница 6-й зоны+2
            return;
        }
        /// <summary>
        /// Метод выводит информацию о файле в экземпляр System.Windows.Forms.Textbox
        /// </summary>
        /// <param name="txt"></param>
        public void ToString(TextBox txt)
        {
            txt.Clear();
            txt.AppendText( "Количество строк = " + this.height + '\n' );
            txt.AppendText( "Количество Столбцов = " + this.width + '\n' );
            for (int i = 0; i < 6; i++) txt.AppendText( "kg" + (i + 1) + " = " + this.kg[i] + "(" + this.logical_zone[i] + ")" + '\n' );
            for (int i = 0; i < 6; i++) txt.AppendText( "kbsb" + (i + 1) + " = " + this.kbsb[i] + '\n' );
            txt.AppendText( "kbst=" + this.kbst + '\n' );
            txt.AppendText( "kdchk=" + this.kdchk + '\n' );
            txt.AppendText( "KS1=" + this.KS1 + '\n' );
            txt.AppendText( "KS2=" + this.KS2 + '\n' );
            txt.AppendText( "LK1=" + this.LK1 + '\n' );
            txt.AppendText( "LK2=" + this.LK2 + '\n' );
            for (int i = 0; i < 4; i++) txt.AppendText("amp" + (i + 1) + "1 = " + (this.amp1[i]) + '\n');
            for (int i = 0; i < 4; i++) txt.AppendText("amp" + (i + 1) + "2 = " + (this.amp2[i]) + '\n');
            txt.AppendText( "KV=" + this.kodeV + '\n' );
            txt.AppendText( "TempK=" + this.temp_k + '\n' );
            txt.AppendText( "TempM=" + this.temp_m + '\n' );
            txt.AppendText( "Кадры=" + this.kadri + '\n' );
            if (this.frags != null && this.kol_frags > 0)
            {
                txt.AppendText("Количество фрагментов в кадре: " + this.kol_frags + '\n');
                for (int i = 0; i < this.kol_frags; i++)
                {
                    txt.AppendText(String.Format("Фрагмент {0}: X={1} Y={2}, X={3} Y={4}", i + 1, frags[i][0], frags[i][1], frags[i][2], frags[i][3]) + '\n');
                }
            }
            return;
        }
        /// <summary>
        /// Метод копирует информацию из экземпляра класса otkuda в экземпляр класса kyda
        /// </summary>
        /// <param name="otkuda">Экземпляр класса, откуда копируются данные</param>
        /// /// <param name="kyda">Экземпляр класса, куда копируются данные</param>
        public void toidz(IDZ1 otkuda, IDZ1 kyda)
        {
            kyda.width = otkuda.width;
            kyda.height = otkuda.height;
            kyda.kg = otkuda.kg;
            kyda.kbsb = otkuda.kbsb;
            kyda.kbst = otkuda.kbst;
            kyda.kdchk = otkuda.kdchk;
            kyda.KS1 = otkuda.KS1;
            kyda.KS2 = otkuda.KS2;
            kyda.LK1 = otkuda.LK1;
            kyda.LK2 = otkuda.LK2;
            kyda.amp1 = otkuda.amp1;
            kyda.amp2 = otkuda.amp2;
            kyda.temp_k = otkuda.temp_k;
            kyda.temp_m = otkuda.temp_m;
            kyda.logical_zone = otkuda.logical_zone;
            kyda.kadri = otkuda.kadri;
            kyda.bytes_per_frame = otkuda.bytes_per_frame;
            kyda.kodeV = otkuda.kodeV;
        }
        /// <summary>
        /// Метод загружает информацию из файла, находящегося в пути path, в экземпляр класса IDZ1
        /// </summary>
        /// <param name="_path">Путь к файлу IDZ или IDZ1</param>
        /// <returns>Экземпляр класса IDZ1, в который будет загружена информация из файла</returns>
        public void loadfromfileidz(string _path)
        {
            int j;
            long size;
            this.path = _path.ToLower();
            if (Path.GetExtension(path) != ".idz" && Path.GetExtension(path) != ".idz1") throw new FileLoadException("Формат файла должен быть idz или idz1!");
            using (BinaryReader fs = new BinaryReader(File.Open(path,FileMode.Open)))
            {
                switch (Path.GetExtension(_path))
                {
                    #region idz
                    case ".idz":
                        {
                            this.format = "idz";
                            size = fs.BaseStream.Length;
                            this.width = fs.ReadUInt16();
                            this.height = fs.ReadByte();
                            //if (modif == false) this.height += 2;
                            for (int i = 0; i < 6; i++)
                            {
                                this.kg[i] = fs.ReadByte();
                            }
                            for (int i = 0; i < 6; i += 2)
                            {
                                j = (fs.ReadByte());
                                this.kbsb[i] = Convert.ToByte((j & 0xF0) >> 4);
                                this.kbsb[i + 1] = Convert.ToByte(j & 0xF);
                            }
                            j = (fs.ReadByte());
                            this.kbst = Convert.ToByte((j & 0xF0) >> 4);
                            this.kdchk = Convert.ToByte(j & 0xF);
                            this.KS1 = fs.ReadByte();
                            this.KS2 = fs.ReadByte();
                            j = (fs.ReadByte());
                            this.LK2 = Convert.ToBoolean(j & 0x2);
                            this.LK1 = Convert.ToBoolean(j & 0x1);
                            for (int i = 0; i < 4; i++)
                            {
                                this.amp1[i] = fs.ReadUInt16();
                                this.amp1[i] = Convert.ToUInt16((this.amp1[i] >> 8) | ((this.amp1[i] & 0xFF) << 8));
                            }
                            this.bytes_per_frame = Convert.ToUInt32(((this.height + 2) * this.width) * 2 + 4);//+2 строки перескана
                            this.kadri = Convert.ToUInt32((size - 16417) / (this.bytes_per_frame));
                            fs.BaseStream.Position = 16417; //пропускаем 8 байт резерва + 16385 байт битового массива кадров
                            fs.BaseStream.Position += this.kadri * this.bytes_per_frame;
                            this.kodeV = 1;
                            this.KS1--;
                            this.KS2--;
                            break;
                        }
                    #endregion
                    #region idz1
                    case ".idz1":
                        {
                            this.format = "idz1";
                            size = fs.BaseStream.Length;
                            this.width = fs.ReadUInt16();
                            this.height = fs.ReadByte();
                            for (int i = 0; i < 6; i++)
                            {
                                this.kg[i] = fs.ReadByte();
                            }
                            for (int i = 0; i < 6; i += 2)
                            {
                                j = (fs.ReadByte());
                                this.kbsb[i] = Convert.ToByte((j & 0xF0) >> 4);
                                this.kbsb[i + 1] = Convert.ToByte(j & 0xF);
                            }
                            j = (fs.ReadByte());
                            this.kbst = Convert.ToByte((j & 0xF0) >> 4);
                            this.kdchk = Convert.ToByte(j & 0xF);
                            this.KS1 = fs.ReadByte();
                            this.KS2 = fs.ReadByte();
                            j = (fs.ReadByte());
                            this.LK2 = Convert.ToBoolean(j & 0x2);
                            this.LK1 = Convert.ToBoolean(j & 0x1);
                            for (int i = 0; i < 4; i++)
                            {
                                this.amp1[i] = fs.ReadUInt16();
                                //this.amp1[i] = Convert.ToUInt16((this.amp1[i] >> 8) | (this.amp1[i] & 0xFF));
                            }
                            for (int i = 0; i < 4; i++)
                            {
                                this.amp2[i] = fs.ReadUInt16();
                                //this.amp2[i] = Convert.ToUInt16((this.amp2[i] >> 8) | (this.amp2[i] & 0xFF));
                            }
                            this.kodeV = fs.ReadUInt16();
                            this.temp_m = fs.ReadInt16();
                            this.temp_k = fs.ReadInt16();
                            if (this.height == 0) this.height = fs.ReadUInt16(); else fs.ReadUInt16();
                            fs.ReadUInt32();
                            this.bytes_per_frame = Convert.ToUInt32(this.height * this.width * 2);
                            this.kadri = Convert.ToUInt32((size - 44) / (this.bytes_per_frame)); //на кадр отводится h*w пикселей, 1 пиксель 2 байта     
                            fs.BaseStream.Position += this.kadri * this.bytes_per_frame;

                            long b = fs.BaseStream.Length - fs.BaseStream.Position; //читаем "хвост" файла если он есть
                            if (b > 0)
                            {
                                kol_frags = fs.ReadUInt16();
                                frags = new ushort[this.kol_frags][];
                                for (int i = 0; i < this.kol_frags; i++)
                                {
                                    frags[i] = new ushort[4];
                                    frags[i][0] = fs.ReadUInt16();
                                    frags[i][1] = fs.ReadUInt16();
                                    frags[i][2] = fs.ReadUInt16();
                                    frags[i][3] = fs.ReadUInt16();
                                }
                            }
                            break;
                        } 
                    #endregion
                }
                
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                if (fs.BaseStream.Position != fs.BaseStream.Length)
                {
                    MessageBox.Show("Файл " + path + " поврежден или имеет неверный формат! Пожалуйста, загрузите другой файл! l=" + 
                        fs.BaseStream.Length + " p=" + fs.BaseStream.Position,"Ошибка файла", buttons, MessageBoxIcon.Error );
                    throw new EndOfStreamException();
                }
            }   //Binaryreader
            this.log_zones();
            return;
        } // конец процедуры loadfromfileidz
        /// <summary>
        /// Метод загружает указанный кадр из указанного файла экземпляра класса IDZ1 в указанный массив pixel_kadr
        /// </summary>
        /// <param name="k">Номер кадра загрузки 1..X</param>
        /// <returns>Массив пикселей, представляющий собой загруженный кадр</returns>
        public int[,] load_frame(int k)
        {
            if (k > this.kadri) throw new ArgumentOutOfRangeException("Кадра с таким номером не существует");
            int[,] pixel_kadr = new int[this.width, this.height];
            using (BinaryReader fs = new BinaryReader(File.Open(this.path,FileMode.Open)))
            {
                if (this.format == "idz")
                {
                    fs.BaseStream.Position = Convert.ToInt64(16417 + ((k - 1) * this.bytes_per_frame));
                    fs.BaseStream.Position += 4;
                }
                else
                {
                    fs.BaseStream.Position = Convert.ToInt64( 44 + ((k - 1) * this.bytes_per_frame) );
                }
                try
                {
                    //byte[] b = fs.ReadBytes((int)this.bytes_per_frame);
                    for (int i = 0; i < this.height; i++) // Цикл строк
                    {
                        if (this.format == "idz" && (i == this.KS1)) fs.BaseStream.Position += this.width * 4;
                        for (int j = 0; j < this.width; j++) pixel_kadr[j, i] = fs.ReadUInt16(); // цикл пикселов(по столбцам)
                    } //for (int i = 0; i < this.height; i++) // Цикл строк
                }
                catch (EndOfStreamException)
                {
                    MessageBox.Show("Файл " + this.path + " поврежден! " + fs.BaseStream.Length + " " + fs.BaseStream.Position,
                                    "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new EndOfStreamException();
                }
                //MessageBox.Show(fs.Length + " " + fs.Position, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } //binaryreader
            return pixel_kadr;
        } //LoadFrameSpectral конец процедуры

        /// <summary>
        /// Возвращает массив двух строк перескана в файле idz по указанному кадру
        /// </summary>
        /// <param name="k">Кадр, из которого необходимы строки перескана [1..N]</param>
        /// <returns>Строки перескана</returns>
        public int[][] Get_Perescan(int k) {
            if (k > this.kadri) throw new ArgumentOutOfRangeException("Кадра с таким номером не существует");
            int[][] pereskan = new int[2][];
            using (BinaryReader fs = new BinaryReader(File.Open(this.path, FileMode.Open))) {
                if (this.format == "idz") {
                    fs.BaseStream.Position = Convert.ToInt64(16417 + ((k - 1) * this.bytes_per_frame));
                    fs.BaseStream.Position += 4 + this.KS1 * this.width * 2;
                }
                else {
                    MessageBox.Show("Строки перескана доступны только в файле типа idz!", "Ошибка формата", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    throw new ArgumentException("Запрос строк перескана из файла формата Idz1");
                }
                byte[] arr = fs.ReadBytes(this.width * 4);
                for (int str = 0; str < 2; str++) {
                    pereskan[str] = new int[this.width];
                    for (int j = 0; j < this.width; j++) pereskan[str][j] = arr[str * this.width * 2 + j * 2] | arr[str * this.width * 2 + j * 2 + 1] << 8;
                }
            }
            return pereskan;
        }

        /// <summary>
        /// Метод загружает указанный кадр из указанного файла экземпляра класса IDZ1 в указанный массив ushort pixel_kadr
        /// </summary>
        /// <param name="k">Номер кадра загрузки 1..x</param>
        /// <returns>Массив пикселей, представляющий собой загруженный кадр</returns>
        public ushort[,] load_frame_short(int k)
        {
            ushort[,] pixel_kadr = new ushort[this.width, this.height];
            using (BinaryReader fs = new BinaryReader(File.Open(this.path,FileMode.Open)))
            {
                if (this.format == "idz")
                {
                    fs.BaseStream.Position = Convert.ToInt64(16417 + ((k - 1) * this.bytes_per_frame));
                    fs.BaseStream.Position += 4;
                }
                else
                {
                    fs.BaseStream.Position = Convert.ToInt64(44 + ((k - 1) * this.bytes_per_frame));
                }
                try
                {
                    for (int i = 0; i < this.height; i++) // Цикл строк
                    {
                        if (this.format == "idz" && (i == this.KS1)) fs.BaseStream.Position += this.width * 4;
                        for (int j = 0; j < this.width; j++) // цикл пикселов(по столбцам)
                        {
                            pixel_kadr[j, i] = fs.ReadUInt16();
                        } //for (int j = 0; j < this.width; j++) // цикл пикселов(по столбцам)
                    } //for (int i = 0; i < this.height; i++) // Цикл строк
                }
                catch (EndOfStreamException)
                {
                    MessageBox.Show("Файл " + this.path + " поврежден! " + fs.BaseStream.Length + " " + fs.BaseStream.Position,
                                    "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new EndOfStreamException();
                }
                //MessageBox.Show(fs.Length + " " + fs.Position, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } //binaryreader
            return pixel_kadr;
        } //LoadFrameSpectral_short конец процедуры
        /// <summary>
        /// Сохраняет указанные кадры из файла в файл по указанному пути 
        /// </summary>
        /// <param name="path">Путь к файлу, который будет сохранен</param>
        /// <param name="ot">Начальный кадр</param>
        /// <param name="do_k">Конечный кадр</param>
        public void save_IDZ1(string path, int ot, int do_k)
        {
            using (BinaryWriter fss = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                fss.Write(this.width);
                if (this.height > 255) fss.Write((byte)0); else fss.Write((byte)this.height);
                for (int i = 0; i < 6; i++) fss.Write(this.kg[i]);
                fss.Write(Convert.ToByte((this.kbsb[0] << 4) | this.kbsb[1]));
                fss.Write(Convert.ToByte((this.kbsb[2] << 4) | this.kbsb[3]));
                fss.Write(Convert.ToByte((this.kbsb[4] << 4) | this.kbsb[5]));
                fss.Write( Convert.ToByte( this.kbst << 4 | this.kdchk) );
                fss.Write( this.KS1);
                fss.Write(this.KS2);
                fss.Write(Convert.ToByte((Convert.ToByte(this.LK2) << 1) | Convert.ToByte(this.LK1))); //а почему смещение было на 2?
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp1[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp1[i] & 0xFF));
                    fss.Write(amp1[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp2[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp2[i] & 0xFF));
                    fss.Write(amp2[i]);
                }
                fss.Write(this.kodeV);
                fss.Write(this.temp_k);
                fss.Write(this.temp_m);
                fss.Write((ushort)this.height);
                fss.Write((uint)0);
                for (int kdr = ot; kdr <= do_k; kdr++)
                {
                    int[,] pix = this.load_frame(kdr);
                    for (int y = 0; y < this.height; y++)
                    {
                        for (int x = 0; x < this.width; x++)
                        {
                            if (pix[x, y] <= 0) fss.Write( (ushort)0 );
                            else fss.Write((ushort)pix[x, y]);
                        }
                    }
                }
            } //using
        }//saveidz
        /// <summary>
        /// Сохраняет файл по указанному пути с заданными размерами
        /// </summary>
        /// <param name="path">Путь к файлу, который будет сохранен</param>
        /// <param name="xmin">Начальное значение столбца</param>
        /// <param name="xmax">Конечное значение столбца</param>
        /// <param name="ymin">Начальное значение строки</param>
        /// <param name="ymax">Конечное значение строки</param>
        /// <param name="kmin">С какого кадра сохранять</param>
        /// <param name="kmax">По какой кадр сохранять</param>
        public void save_IDZ1(string path, ushort xmin, ushort xmax, ushort ymin, ushort ymax, int kmin=1, int kmax=0) 
        {
            using (BinaryWriter fss = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                if (kmax == 0) kmax = (int)this.kadri;
                ushort w = (ushort)(xmax - xmin +1);
                ushort h = (ushort)(ymax - ymin + 1);
                fss.Write(w);
                if (h > 255) fss.Write((byte)0); else fss.Write((byte)h);
                for (int i = 0; i < 6; i++) fss.Write(this.kg[i]);
                fss.Write(Convert.ToByte((this.kbsb[0] << 4) | this.kbsb[1]));
                fss.Write(Convert.ToByte((this.kbsb[2] << 4) | this.kbsb[3]));
                fss.Write(Convert.ToByte((this.kbsb[4] << 4) | this.kbsb[5]));
                fss.Write(Convert.ToByte(this.kbst << 4 | this.kdchk));
                fss.Write(this.KS1);
                fss.Write(this.KS2);
                fss.Write(Convert.ToByte((Convert.ToByte(this.LK2) << 1) | Convert.ToByte(this.LK1))); //а почему смещение было на 2?
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp1[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp1[i] & 0xFF));
                    fss.Write(amp1[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp2[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp2[i] & 0xFF));
                    fss.Write(amp2[i]);
                }
                fss.Write(this.kodeV);
                fss.Write(this.temp_k);
                fss.Write(this.temp_m);
                fss.Write(h);
                fss.Write((uint)0);
                for (int kdr = kmin; kdr <= kmax; kdr++)
                {
                    int[,] pix = this.load_frame(kdr);
                    for (int y = ymin-1; y < ymax; y++)
                    {
                        for (int x = xmin-1; x < xmax; x++)
                        {
                            if (pix[x,y] <= 0) fss.Write((ushort)0);
                            else fss.Write((ushort)(pix[x, y]));
                        }
                    }
                }
            } //using
        }//saveidz
        /// <summary>
        /// Сохраняет файл по указанному пути
        /// </summary>
        /// <param name="path">Путь к файлу, который будет сохранен</param>
        public void save_IDZ1(string path)
        {
            using (BinaryWriter fss = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                fss.Write(this.width);
                if (this.height > 255) fss.Write((byte)0); else fss.Write((byte)this.height);
                for (int i = 0; i < 6; i++) fss.Write(this.kg[i]);
                fss.Write(Convert.ToByte((this.kbsb[0] << 4) | this.kbsb[1]));
                fss.Write(Convert.ToByte((this.kbsb[2] << 4) | this.kbsb[3]));
                fss.Write(Convert.ToByte((this.kbsb[4] << 4) | this.kbsb[5]));
                fss.Write(Convert.ToByte(this.kbst << 4 | this.kdchk));
                fss.Write(this.KS1);
                fss.Write(this.KS2);
                fss.Write(Convert.ToByte((Convert.ToByte(this.LK2) << 1) | Convert.ToByte(this.LK1))); //а почему смещение было на 2?
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp1[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp1[i] & 0xFF));
                    fss.Write(amp1[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp2[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp2[i] & 0xFF));
                    fss.Write(amp2[i]);
                }
                fss.Write(this.kodeV);
                fss.Write(this.temp_k);
                fss.Write(this.temp_m);
                fss.Write((ushort)this.height);
                fss.Write((uint)0);
                for (int kdr = 1; kdr <= this.kadri; kdr++)
                {
                    int[,] pix = this.load_frame(kdr);
                    for (int y = 0; y < this.height; y++)
                    {
                        for (int x = 0; x < this.width; x++)
                        {
                            if (pix[x, y] <= 0) fss.Write((ushort)0);
                            else fss.Write((ushort)pix[x, y]);
                        }
                    }
                }
            } //using
        }
        /// <summary>
        /// Сохраняет кадр в файл по указанному пути
        /// </summary>
        /// <param name="path">Путь к файлу, который будет сохранен</param>
        /// <param name="frm">Кадр для сохранения[ширина, высота]</param>
        public void save_IDZ1(string path, int[,] frm)
        {
            using (BinaryWriter fss = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                fss.Write(this.width);
                if (this.height > 255) fss.Write((byte)0); else fss.Write((byte)this.height);
                for (int i = 0; i < 6; i++) fss.Write(this.kg[i]);
                fss.Write(Convert.ToByte((this.kbsb[0] << 4) | this.kbsb[1]));
                fss.Write(Convert.ToByte((this.kbsb[2] << 4) | this.kbsb[3]));
                fss.Write(Convert.ToByte((this.kbsb[4] << 4) | this.kbsb[5]));
                fss.Write(Convert.ToByte(this.kbst << 4 | this.kdchk));
                fss.Write(this.KS1);
                fss.Write(this.KS2);
                fss.Write(Convert.ToByte((Convert.ToByte(this.LK2) << 1) | Convert.ToByte(this.LK1))); //а почему смещение было на 2?
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp1[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp1[i] & 0xFF));
                    fss.Write(amp1[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp2[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp2[i] & 0xFF));
                    fss.Write(amp2[i]);
                }
                fss.Write(this.kodeV);
                fss.Write(this.temp_k);
                fss.Write(this.temp_m);
                fss.Write((ushort)this.height);
                fss.Write((uint)0);
                for (int y = 0; y < this.height; y++)
                {
                    for (int x = 0; x < this.width; x++)
                    {
                        if (frm[x, y] <= 0) fss.Write((ushort)0);
                        else fss.Write((ushort)frm[x, y]);
                    }
                }
            } //using
        }
        /// <summary>
        /// Сохраняет гиперкуб данных в файл по указанному пути
        /// </summary>
        /// <param name="path">Путь к файлу, который будет сохранен</param>
        /// <param name="frms">Гиперкуб для сохранения[кадр][ширина, высота]</param>
        public void save_IDZ1(string path, int[][,] frms)
        {
            using (BinaryWriter fss = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                fss.Write(this.width);
                if (this.height > 255) fss.Write((byte)0); else fss.Write((byte)this.height);
                for (int i = 0; i < 6; i++) fss.Write(this.kg[i]);
                fss.Write(Convert.ToByte((this.kbsb[0] << 4) | this.kbsb[1]));
                fss.Write(Convert.ToByte((this.kbsb[2] << 4) | this.kbsb[3]));
                fss.Write(Convert.ToByte((this.kbsb[4] << 4) | this.kbsb[5]));
                fss.Write(Convert.ToByte(this.kbst << 4 | this.kdchk));
                fss.Write(this.KS1);
                fss.Write(this.KS2);
                fss.Write(Convert.ToByte((Convert.ToByte(this.LK2) << 1) | Convert.ToByte(this.LK1))); //а почему смещение было на 2?
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp1[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp1[i] & 0xFF));
                    fss.Write(amp1[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp2[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp2[i] & 0xFF));
                    fss.Write(amp2[i]);
                }
                fss.Write(this.kodeV);
                fss.Write(this.temp_k);
                fss.Write(this.temp_m);
                fss.Write((ushort)this.height);
                fss.Write((uint)0);
                for (int f = 0; f < frms.Length; f++)
                {
                    for (int y = 0; y < this.height; y++)
                    {
                        for (int x = 0; x < this.width; x++)
                        {
                            if (frms[f][x, y] <= 0) fss.Write((ushort)0);
                            else fss.Write((ushort)frms[f][x, y]);
                        } //for x
                    } //for y
                }//for f
            } //using
        }
        /// <summary>
        /// Сохраняет гиперкуб данных USHORT в файл по указанному пути
        /// </summary>
        /// <param name="path">Путь к файлу, который будет сохранен</param>
        /// <param name="frms">Гиперкуб для сохранения[кадр][ширина, высота]</param>
        public void save_IDZ1(string path, ushort[][,] frms)
        {
            using (BinaryWriter fss = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                fss.Write(this.width);
                if (this.height > 255) fss.Write((byte)0); else fss.Write((byte)this.height);
                for (int i = 0; i < 6; i++) fss.Write(this.kg[i]);
                fss.Write(Convert.ToByte((this.kbsb[0] << 4) | this.kbsb[1]));
                fss.Write(Convert.ToByte((this.kbsb[2] << 4) | this.kbsb[3]));
                fss.Write(Convert.ToByte((this.kbsb[4] << 4) | this.kbsb[5]));
                fss.Write(Convert.ToByte(this.kbst << 4 | this.kdchk));
                fss.Write(this.KS1);
                fss.Write(this.KS2);
                fss.Write(Convert.ToByte((Convert.ToByte(this.LK2) << 1) | Convert.ToByte(this.LK1))); //а почему смещение было на 2?
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp1[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp1[i] & 0xFF));
                    fss.Write(amp1[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp2[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp2[i] & 0xFF));
                    fss.Write(amp2[i]);
                }
                fss.Write(this.kodeV);
                fss.Write(this.temp_k);
                fss.Write(this.temp_m);
                fss.Write((ushort)this.height);
                fss.Write((uint)0);
                for (int f = 0; f < frms.Length; f++)
                {
                    for (int y = 0; y < this.height; y++)
                    {
                        for (int x = 0; x < this.width; x++)
                        {
                            if (frms[f][x, y] <= 0) fss.Write((ushort)0);
                            else fss.Write(frms[f][x, y]);
                        } //for x
                    } //for y
                }//for f
            } //using
        }
        /// <summary>
        /// Сохраняет файл по указанному пути, перемещая данные из указанного файла с указанным смещением на данные
        /// </summary>
        /// <param name="path">Путь к сохраняемому файлу</param>
        /// <param name="pathrootfile">Путь к файлу, откуда копируется информация</param>
        /// <param name="offset">Смещение в байтах, откуда начинаются данные</param>
        /// <param name="byte_per_digit">Количество байт на число</param>
        public void save_IDZ1(string path, string pathrootfile, int offset, byte byte_per_digit)
        {
            using (BinaryWriter fss = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                fss.Write(this.width);
                if (this.height > 255) fss.Write((byte)0); else fss.Write((byte)this.height);
                for (int i = 0; i < 6; i++) fss.Write(this.kg[i]);
                fss.Write(Convert.ToByte((this.kbsb[0] << 4) | this.kbsb[1]));
                fss.Write(Convert.ToByte((this.kbsb[2] << 4) | this.kbsb[3]));
                fss.Write(Convert.ToByte((this.kbsb[4] << 4) | this.kbsb[5]));
                fss.Write(Convert.ToByte(this.kbst << 4 | this.kdchk));
                fss.Write(this.KS1);
                fss.Write(this.KS2);
                fss.Write(Convert.ToByte((Convert.ToByte(this.LK2) << 1) | Convert.ToByte(this.LK1))); //а почему смещение было на 2?
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp1[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp1[i] & 0xFF));
                    fss.Write(amp1[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp2[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp2[i] & 0xFF));
                    fss.Write(amp2[i]);
                }
                fss.Write(this.kodeV);
                fss.Write(this.temp_k);
                fss.Write(this.temp_m);
                fss.Write((ushort)this.height);
                fss.Write((uint)0);
                using (FileStream origin = new FileStream(pathrootfile, FileMode.Open, FileAccess.Read))
                {
                    int data = Convert.ToInt32(this.kadri * this.height * this.width * byte_per_digit);
                    if (data > origin.Length && data / 2 == origin.Length - offset) data /= 2;  //проверка при ошибке считывания пикселов
                    origin.Position = offset;                                                   //из файла может считаться 1 бит на пиксел, по факту
                    byte[] mass = new byte[data];                                               //2 байта на пиксел, поэтому размер блока данных 
                    origin.Read(mass, 0, data);                                                 //уменьшается в 2 раза(в пикселах)
                    if (byte_per_digit == 1)
                    {
                        Array.Resize(ref mass, mass.Length * 2);
                        for (int i = mass.Length / 2; i > 1; i--)
                        {
                            mass[i * 2 - 2] = mass[i - 1];
                            mass[i - 1] = 0;
                        }
                    }
                    fss.Write(mass,0,mass.Length);
                }
            } //using
        }
        /// <summary>
        /// Сохраняет в файл Idz1 кадр и координаты фреймов
        /// </summary>
        /// <param name="path">Путь к сохраняемому файлу</param>
        /// <param name="kadr">Кадр информации[ширина, высота]</param>
        /// <param name="frames">Координаты фрэймов[i]{x1,y1,x2,y2}</param>
        public void save_IDZ1(string path, ushort[,] kadr, ref ushort[][] frames)
        {
            using (BinaryWriter fss = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                fss.Write(this.width);
                if (this.height > 255) fss.Write((byte)0); else fss.Write((byte)this.height);
                for (int i = 0; i < 6; i++) fss.Write(this.kg[i]);
                fss.Write(Convert.ToByte((this.kbsb[0] << 4) | this.kbsb[1]));
                fss.Write(Convert.ToByte((this.kbsb[2] << 4) | this.kbsb[3]));
                fss.Write(Convert.ToByte((this.kbsb[4] << 4) | this.kbsb[5]));
                fss.Write(Convert.ToByte(this.kbst << 4 | this.kdchk));
                fss.Write(this.KS1);
                fss.Write(this.KS2);
                fss.Write(Convert.ToByte((Convert.ToByte(this.LK2) << 1) | Convert.ToByte(this.LK1))); //а почему смещение было на 2?
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp1[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp1[i] & 0xFF));
                    fss.Write(amp1[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp2[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp2[i] & 0xFF));
                    fss.Write(amp2[i]);
                }
                fss.Write(this.kodeV);
                fss.Write(this.temp_k);
                fss.Write(this.temp_m);
                fss.Write((ushort)this.height);
                fss.Write((uint)0);
                for (int y = 0; y < this.height; y++)
                {
                    for (int x = 0; x < this.width; x++)
                    {
                        if (kadr[x, y] <= 0) fss.Write((ushort)0);
                        else fss.Write((ushort)kadr[x, y]);
                    }
                }
                fss.Write((ushort)frames.Length);
                for (int i = 0; i < frames.Length; i++)
                {
                    fss.Write(frames[i][0]);
                    fss.Write(frames[i][1]);
                    fss.Write(frames[i][2]);
                    fss.Write(frames[i][3]);
                }
            }
        }
        /// <summary>
        /// Сохраняет данные экземпляра IDZ1 в файл для последующей дозаписи кадров
        /// </summary>
        /// <param name="path">Путь к сохраняемому файлу</param>
        public void save_head(string path) 
        {
            using (BinaryWriter fss = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                fss.Write(this.width);
                if (this.height > 255) fss.Write((byte)0); else fss.Write((byte)this.height);
                for (int i = 0; i < 6; i++) fss.Write(this.kg[i]);
                fss.Write(Convert.ToByte((this.kbsb[0] << 4) | this.kbsb[1]));
                fss.Write(Convert.ToByte((this.kbsb[2] << 4) | this.kbsb[3]));
                fss.Write(Convert.ToByte((this.kbsb[4] << 4) | this.kbsb[5]));
                fss.Write(Convert.ToByte(this.kbst << 4 | this.kdchk));
                fss.Write(this.KS1);
                fss.Write(this.KS2);
                fss.Write(Convert.ToByte((Convert.ToByte(this.LK2) << 1) | Convert.ToByte(this.LK1))); //а почему смещение было на 2?
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp1[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp1[i] & 0xFF));
                    fss.Write(amp1[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    //fss.Write(Convert.ToByte((amp2[i] & 0xFF00) >> 8));
                    //fss.Write(Convert.ToByte(amp2[i] & 0xFF));
                    fss.Write(amp2[i]);
                }
                fss.Write(this.kodeV);
                fss.Write(this.temp_k);
                fss.Write(this.temp_m);
                fss.Write((ushort)this.height);
                fss.Write((uint)0);
            } //using
        }
        /// <summary>
        /// Записывает в конец файла указанный кадр в виде массива byte[]
        /// </summary>
        /// <param name="path">Путь к файлу для записи</param>
        /// <param name="frame">Кадр</param>
        public void Write_frame(string path, byte[] frame)
        {
            using (FileStream fss = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                fss.Write(frame,0,frame.Length);
            } //using
        }
        /// <summary>
        /// Записывает в конец файла указанный кадр в виде массива ushort[,]
        /// </summary>
        /// <param name="path">Путь к файлу для записи</param>
        /// <param name="frame">Кадр[ширина, высота]</param>
        public void Write_frame(string path, ushort[,] frame)
        {
            using (BinaryWriter fss = new BinaryWriter(File.Open(path, FileMode.Append, FileAccess.Write)))
            {
                for (int j = 0; j < this.height; j++)
                {
                    for (int i = 0; i < this.width; i++) fss.Write(frame[i,j]);
                }
            } //using
        }
        /// <summary>
        /// Записывает в конец файла указанный кадр в виде массива int[,]
        /// </summary>
        /// <param name="path">Путь к файлу для записи</param>
        /// <param name="frame">Кадр[ширина, высота]</param>
        public void Write_frame(string path, int[,] frame) {
            using (BinaryWriter fss = new BinaryWriter(File.Open(path, FileMode.Append, FileAccess.Write))) {
                for (int j = 0; j < this.height; j++) {
                    for (int i = 0; i < this.width; i++) fss.Write((short)frame[i, j]);
                }
            } //using
        }
    }
}