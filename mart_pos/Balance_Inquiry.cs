using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mart_pos
{
    public partial class Balance_Inquiry : Form
    {
        BalanceDB Bd = new BalanceDB(); // 디비 클래스 생성
        public Balance_Inquiry()
        {
            InitializeComponent();
            Bd.Table_insert(dataGridView1); // 생성과 동시에 테이블 데이터 업데이트
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("포스기에 추가 할 금액을 입력해주세요.", "금액 추가", "추가할 금액을 입력하세요.");

            if(input != null)
            {
                if (int.TryParse(input, out int n)) // 입력한 데이터가 숫자가 맞는 경우
                {
                    Bd.price_append(n, dataGridView1); // 받은 데이터를 디비와 현재 데이터 그리드뷰에 업데이트
                }
                else
                {
                    MessageBox.Show("숫자를 입력하세요.");
                }
            }
            
                
        }

        // 뒤로버튼
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // 현재 창 닫기
            Bd.dbclose(); // 디비 종료
        }


        // 창을 닫으면서 디비도 함께 종료
        private void Balance_Inquiry_FormClosing(object sender, FormClosingEventArgs e)
        {
            Bd.dbclose(); // 디비 종료
        }

    }
}
