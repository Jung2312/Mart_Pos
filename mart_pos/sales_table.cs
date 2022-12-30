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
    public partial class sales_table : Form
    {
        SalesDB Sd = new SalesDB(); // 데이터 베이스를 사용하기 위한 클래스 생성
        public sales_table()
        {
            
            InitializeComponent();
            Sd.Table_insert(dataGridView1); // 테이블에 데이터 채우는 메소드 사용
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close(); // 현재 창 닫기
            Sd.dbclose(); // 디비 종료
        }

        private void sales_table_FormClosing(object sender, FormClosingEventArgs e)
        {
            Sd.dbclose();
        }
    }
}
