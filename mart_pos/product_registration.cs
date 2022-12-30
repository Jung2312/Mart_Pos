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
    public partial class product_registration : Form
    {
        ProductDB Pd = new ProductDB();
        public product_registration()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // 현재 창 닫기
            Pd.dbclose(); // 디비 종료
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(textBox1.Text + " 을(를) 등록 하시겠습니까?", "상품 등록", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (String.IsNullOrWhiteSpace(textBox1.Text))  // 상품명 미입력(공백, 스페이스 모두 다 판별)
                {
                    MessageBox.Show("상품명을 입력하세요.");
                    textBox1.Focus(); // 상품명 칸에 포커스
                }

                else if (String.IsNullOrWhiteSpace(textBox2.Text)) // 단가 미입력
                {
                    MessageBox.Show("단가를 입력하세요.");
                    textBox2.Focus(); // 단가에 포커스
                }

                else if (String.IsNullOrWhiteSpace(textBox1.Text) && String.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("상품명과 단가를 입력하세요.");
                }

                else
                {
                    if (int.TryParse(textBox2.Text, out int n))
                    {
                        if (Pd.Pro_insert(textBox1.Text, textBox2.Text)) // 상품 메소드 만들것 이름 비교
                        {
                            MessageBox.Show("상품이 등록되었습니다.");
                            textBox1.Text = null;
                            textBox2.Text = null ;
                        }

                        else
                        {
                            MessageBox.Show("이미 존재하는 상품입니다.");
                            textBox1.Text = null;
                        }
                    }

                    else
                    {
                        MessageBox.Show("수량 입력이 취소 되었습니다.");
                        textBox2.Text = null;
                    }

                }


            }
            else
            {
                MessageBox.Show("등록을 취소했습니다.");
            }
        }

        private void product_registration_FormClosing(object sender, FormClosingEventArgs e)
        {
            Pd.dbclose();
        }
    }
}
