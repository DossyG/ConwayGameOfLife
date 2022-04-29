using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConwayGameOfLife
{
    public partial class OptionsDialog : Form
    {
        public OptionsDialog()
        {
            InitializeComponent();
        }

        public int timerInterval
        {
            get
            {
                return (int)numericUpDownTimer.Value;
            }
            set
            {
                numericUpDownTimer.Value = value;
            }
        }

        public int universeWidth
        {
            get
            {
                return (int)numericUpDownWidth.Value;
            }
            set
            {
                numericUpDownWidth.Value = value;
            }
        }

        public int universeHeight
        {
            get
            {
                return (int)numericUpDownHeight.Value;
            }
            set
            {
                numericUpDownHeight.Value = value;
            }
        }
    }
}
