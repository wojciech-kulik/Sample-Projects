using SampleDLL;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace SampleApp
{
    public partial class Form1 : Form
    {
        OpenFileDialog openDialog = new OpenFileDialog();
        SaveFileDialog saveDialog = new SaveFileDialog();

        public Form1()
        {
            InitializeComponent();
        }

        private void Serialize(object obj)
        {
            if (saveDialog.ShowDialog(this) == DialogResult.OK)
            {
                using (var stream = new FileStream(saveDialog.FileName, FileMode.Create))
                {
                    new BinaryFormatter().Serialize(stream, obj);
                }
            }
        }

        private object Deserialize()
        {
            if (openDialog.ShowDialog(this) == DialogResult.OK)
            {
                using (var stream = new FileStream(openDialog.FileName, FileMode.Open))
                {
                    return new BinaryFormatter().Deserialize(stream);
                }
            }
            return null;
        }

        private void btnSerializeWrong_Click(object sender, EventArgs e)
        {
            Serialize(new InternalClass());
        }

        private void btnSerializeOK_Click(object sender, EventArgs e)
        {
            Serialize(new ExternalClass());
        }

        private void btnDeserialize_Click(object sender, EventArgs e)
        {
            Deserialize();
        }
    }
}
