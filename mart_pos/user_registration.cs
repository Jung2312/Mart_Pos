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
    public partial class user_registration : Form
    {
        UserDB Ud = new UserDB();
        public user_registration()
        {
            InitializeComponent();
            textBox2.Text = "010";
        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // 현재 창 닫기
            Ud.dbclose(); // 디비 종료

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show(textBox1.Text + " 회원님을 등록 하시겠습니까?", "회원 등록", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                if(String.IsNullOrWhiteSpace(textBox1.Text))  // 이름 미입력(공백, 스페이스 모두 다 판별)
                {
                    MessageBox.Show("회원명을 입력하세요.");
                    textBox1.Focus(); // 이름 칸에 포커스
                }

                else if(String.IsNullOrWhiteSpace(textBox2.Text)) // 전화번호 미입력
                {
                    MessageBox.Show("전화번호를 입력하세요.");
                    textBox2.Focus(); // 전화 번호에 포커스
                }

                else if(String.IsNullOrWhiteSpace(textBox1.Text) && String.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("이름과 전화번호를 입력하세요.");
                }

                else if(int.TryParse(textBox1.Text , out int result) != false)
                {
                    MessageBox.Show("숫자를 입력하세요.");
                }

                else
                {
                    if (textBox2.Text.Length == 11)
                    {
                        if (Ud.User_insert(textBox1.Text, textBox2.Text))
                        {
                            MessageBox.Show("회원이 등록되었습니다.");
                            textBox1.Text = null;
                            textBox2.Text = "010";
                        }

                        else
                        {
                            MessageBox.Show("이미 존재하는 회원입니다.");
                            textBox2.Text = "010";
                        }
                    }

                    else
                    {
                        MessageBox.Show("전화 번호는 11자리로 입력하세요.\n(ex. 010 0000 0000 공백 제외)");
                        textBox2.Text = "010";
                    }
                }
                
                
            }
            else
            {
                MessageBox.Show("회원 등록이 취소 되었습니다.");
            }
            
        }

        private void user_registration_FormClosing(object sender, FormClosingEventArgs e)
        {
            Ud.dbclose();
        }
    }
}
