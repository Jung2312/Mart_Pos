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
    public partial class user_table : Form
    {
        UserDB Ud = new UserDB(); // 데이터 베이스를 사용하기 위한 클래스 생성
        public user_table()
        {
            InitializeComponent();
            Ud.Table_insert(dataGridView1); // 테이블 채우는 메소드 사용
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // 현재 창 닫기
            Ud.dbclose(); // 디비 종료
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Ud.User_search(textBox1.Text, dataGridView1);
            textBox1.Text = ""; // 검색 완료 후 초기화


        }

        private void button1_Click(object sender, EventArgs e)
        {
            user us = new user();
            user2 us2 = new user2();
            dataGridView1.EndEdit();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((Convert.ToBoolean(row.Cells[0].Value) == true))
                {
                    us[us.Check] = row.Cells[2].Value.ToString(); // us 클래스의 삭제하고자 하는 유저 전화번호 인덱서에 값을 저장
                    us2[us.Check] = row.Cells[1].Value.ToString();
                    us.Check += 1;
                    us2.Check += 1;
                }

            }

            for (int i = 0; i < us.size(); i++)

            {
                if (MessageBox.Show(us2[i] + " 회원님을 삭제 하시겠습니까?", "회원 삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Ud.User_delete(us[i], dataGridView1); // 확인을 누를 경우 데이터 삭제
                }
                else
                {
                    MessageBox.Show("삭제를 취소하셨습니다.");
                }

            }
        }

        private void user_table_FormClosing(object sender, FormClosingEventArgs e)
        {
            Ud.dbclose();
        }
    }
}
