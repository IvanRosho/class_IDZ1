using System;
using System.Drawing;
using System.Windows.Forms;

namespace IDZ1_library
{
    /// <summary>
    /// Класс формы для отображения информации о изображении TIFF
    /// </summary>
    internal class INFO_TIF : Form
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Text_Descriptor;
        private System.Windows.Forms.TextBox Text_Version;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox Text_TagNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox List_tag;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox List_Data;
        private System.Windows.Forms.Label label7;
        private TIFF tif;
        Label[][] Labels;
        TextBox[][] TextBoxes;
        uint[] offsets;
        /// <summary>
        /// Конструктор формы
        /// </summary>
        /// <param name="t">Экземпляр класса TIFF, информация о котором будет отображена</param>
        internal INFO_TIF(TIFF t)
        {
            InitializeComponent();
            Labels = new Label[4][];
            TextBoxes = new TextBox[6][];
            tif = t;
            this.Show();
        }
        /// <summary>
        /// Освобождение ресурса
        /// </summary>
        /// <param name="disposing">Освобождать или нет?</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Text_Descriptor = new System.Windows.Forms.TextBox();
            this.Text_Version = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Text_TagNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.List_tag = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.List_Data = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Дескриптор";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Версия";
            // 
            // Text_Descriptor
            // 
            this.Text_Descriptor.BackColor = System.Drawing.Color.LightCyan;
            this.Text_Descriptor.Location = new System.Drawing.Point(121, 6);
            this.Text_Descriptor.Name = "Text_Descriptor";
            this.Text_Descriptor.ReadOnly = true;
            this.Text_Descriptor.Size = new System.Drawing.Size(100, 20);
            this.Text_Descriptor.TabIndex = 5;
            // 
            // Text_Version
            // 
            this.Text_Version.BackColor = System.Drawing.Color.LightCyan;
            this.Text_Version.Location = new System.Drawing.Point(121, 35);
            this.Text_Version.Name = "Text_Version";
            this.Text_Version.ReadOnly = true;
            this.Text_Version.Size = new System.Drawing.Size(100, 20);
            this.Text_Version.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 92);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(806, 168);
            this.panel1.TabIndex = 8;
            // 
            // Text_TagNumber
            // 
            this.Text_TagNumber.BackColor = System.Drawing.Color.LightCyan;
            this.Text_TagNumber.Location = new System.Drawing.Point(336, 6);
            this.Text_TagNumber.Name = "Text_TagNumber";
            this.Text_TagNumber.ReadOnly = true;
            this.Text_TagNumber.Size = new System.Drawing.Size(100, 20);
            this.Text_TagNumber.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(227, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Количество тегов";
            // 
            // List_tag
            // 
            this.List_tag.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.List_tag.FormattingEnabled = true;
            this.List_tag.Location = new System.Drawing.Point(445, 22);
            this.List_tag.Name = "List_tag";
            this.List_tag.Size = new System.Drawing.Size(120, 56);
            this.List_tag.TabIndex = 13;
            this.List_tag.SelectedIndexChanged += new System.EventHandler(this.List_tag_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(442, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Данные По тегу";
            // 
            // List_Data
            // 
            this.List_Data.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.List_Data.FormattingEnabled = true;
            this.List_Data.Location = new System.Drawing.Point(584, 22);
            this.List_Data.Name = "List_Data";
            this.List_Data.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.List_Data.Size = new System.Drawing.Size(210, 56);
            this.List_Data.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(581, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Данные тега";
            // 
            // Info_tif
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 260);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.List_Data);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.List_tag);
            this.Controls.Add(this.Text_TagNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Text_Version);
            this.Controls.Add(this.Text_Descriptor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Info_tif";
            this.Text = "Информация о файле tif";
            this.Shown += new System.EventHandler(this.Info_tif_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private void Info_tif_Shown(object sender, EventArgs e)
        {
            try
            {
                this.Text = "Информация о файле " + tif.path;
                Text_Descriptor.Text = tif.byte_order == true ? "II" : "MM";
                Text_Version.Text = tif.version.ToString();
                Text_TagNumber.Text = Convert.ToString(tif.Num_Tags);
                offsets = new uint[tif.Num_Tags];
                Labels[0] = new Label[tif.Num_Tags];
                Labels[1] = new Label[tif.Num_Tags];
                Labels[2] = new Label[tif.Num_Tags];
                Labels[3] = new Label[tif.Num_Tags];
                TextBoxes[0] = new TextBox[tif.Num_Tags];
                TextBoxes[1] = new TextBox[tif.Num_Tags];
                TextBoxes[2] = new TextBox[tif.Num_Tags];
                TextBoxes[3] = new TextBox[tif.Num_Tags];
                TextBoxes[4] = new TextBox[tif.Num_Tags];
                TextBoxes[5] = new TextBox[tif.Num_Tags];
                for (int i = 0; i < tif.Num_Tags; i++)
                {
                    #region динамическая разметка
                    Labels[0][i] = new Label();
                    Labels[1][i] = new Label();
                    Labels[2][i] = new Label();
                    Labels[3][i] = new Label();
                    Labels[0][i].Location = new Point(10, 10 + (i * 25));
                    Labels[1][i].Location = new Point(225, 10 + (i * 25));
                    Labels[2][i].Location = new Point(410, 10 + (i * 25));
                    Labels[3][i].Location = new Point(575, 10 + (i * 25));
                    Labels[0][i].Text = "Тег";
                    Labels[1][i].Text = "Значение, бит";
                    Labels[2][i].Text = "Количество значений";
                    Labels[3][i].Text = "Указатель";
                    Labels[0][i].Visible = true;
                    Labels[1][i].Visible = true;
                    Labels[2][i].Visible = true;
                    Labels[3][i].Visible = true;
                    Labels[0][i].AutoSize = true;
                    Labels[1][i].AutoSize = true;
                    Labels[2][i].AutoSize = true;
                    Labels[3][i].AutoSize = true;
                    TextBoxes[0][i] = new TextBox();
                    TextBoxes[1][i] = new TextBox();
                    TextBoxes[2][i] = new TextBox();
                    TextBoxes[3][i] = new TextBox();
                    TextBoxes[4][i] = new TextBox();
                    TextBoxes[5][i] = new TextBox();
                    panel1.Controls.Add(Labels[0][i]);
                    panel1.Controls.Add(Labels[1][i]);
                    panel1.Controls.Add(Labels[2][i]);
                    panel1.Controls.Add(Labels[3][i]);
                    panel1.Controls.Add(TextBoxes[0][i]);
                    panel1.Controls.Add(TextBoxes[1][i]);
                    panel1.Controls.Add(TextBoxes[2][i]);
                    panel1.Controls.Add(TextBoxes[3][i]);
                    panel1.Controls.Add(TextBoxes[4][i]);
                    panel1.Controls.Add(TextBoxes[5][i]);
                    TextBoxes[0][i].Location = new Point(40, 8 + (i * 25));
                    TextBoxes[1][i].Location = new Point(85, 8 + (i * 25));
                    TextBoxes[2][i].Location = new Point(305, 8 + (i * 25));
                    TextBoxes[3][i].Location = new Point(330, 8 + (i * 25));
                    TextBoxes[4][i].Location = new Point(530, 8 + (i * 25));
                    TextBoxes[5][i].Location = new Point(640, 8 + (i * 25));
                    TextBoxes[0][i].Width = 38;
                    TextBoxes[1][i].Width = 135;
                    TextBoxes[2][i].Width = 20;
                    TextBoxes[3][i].Width = 75;
                    TextBoxes[4][i].Width = 40;
                    TextBoxes[5][i].Width = 110;
                    TextBoxes[0][i].ReadOnly = true;
                    TextBoxes[1][i].ReadOnly = true;
                    TextBoxes[2][i].ReadOnly = true;
                    TextBoxes[3][i].ReadOnly = true;
                    TextBoxes[4][i].ReadOnly = true;
                    TextBoxes[5][i].ReadOnly = true;
                    TextBoxes[0][i].BackColor = Color.Azure;
                    TextBoxes[1][i].BackColor = Color.Azure;
                    TextBoxes[2][i].BackColor = Color.Azure;
                    TextBoxes[3][i].BackColor = Color.Azure;
                    TextBoxes[4][i].BackColor = Color.Azure;
                    TextBoxes[5][i].BackColor = Color.Azure;
                    TextBoxes[0][i].Visible = true;
                    TextBoxes[1][i].Visible = true;
                    TextBoxes[2][i].Visible = true;
                    TextBoxes[3][i].Visible = true;
                    TextBoxes[4][i].Visible = true;
                    TextBoxes[5][i].Visible = true;

                    #endregion
                    TextBoxes[0][i].Text = Convert.ToString(tif.Tags[i].tag);
                    TextBoxes[1][i].Text = tif.Tags[i].name;
                    TextBoxes[3][i].Text = tif.Tags[i].type_data;
                    if (tif.Tags[i].type == 1 || tif.Tags[i].type == 2 || tif.Tags[i].type == 6) TextBoxes[2][i].Text = "1";
                    else if (tif.Tags[i].type == 3 || tif.Tags[i].type == 8) TextBoxes[2][i].Text = "2";
                    else if (tif.Tags[i].type == 4 || tif.Tags[i].type == 9 || tif.Tags[i].type == 11) TextBoxes[2][i].Text = "4";
                    else if (tif.Tags[i].type == 5 || tif.Tags[i].type == 10 || tif.Tags[i].type == 12) TextBoxes[2][i].Text = "8";
                    TextBoxes[4][i].Text = tif.Tags[i].count_field.ToString();
                    if (tif.Tags[i].count_field == 1 && TextBoxes[3][i].Text != "URATIONAL" && TextBoxes[3][i].Text != "SRATIONAL")
                    {
                        Labels[3][i].Text = "Значение";
                        TextBoxes[5][i].Text = Convert.ToString(tif.Tags[i].data[0]);
                    }
                    else
                    {
                        List_tag.Items.Add(tif.Tags[i].tag);
                        TextBoxes[5][i].Text = Convert.ToString(tif.Tags[i].pointer);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close();
                return;
            }
        }

        private void List_tag_SelectedIndexChanged(object sender, EventArgs e)
        {
            List_Data.Items.Clear();
            int index = 0;
            for (int i = 0; i < tif.Num_Tags; i++) if (tif.Tags[i].tag == Convert.ToUInt16(List_tag.SelectedItem))
                {
                    index = i; break;
                }
            for (int i = 0; i < int.Parse(TextBoxes[4][index].Text); i++) List_Data.Items.Add(tif.Tags[index].data[i]);
        }
    }
}
