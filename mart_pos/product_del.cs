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
    public partial class product_del : Form
    {
        ProductDB Pd = new ProductDB();
        public product_del()
        {
            InitializeComponent();
            Pd.del_Table(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // 현재 창 닫기
            Pd.dbclose(); // 디비 종료
        }

        private void button1_Click(object sender, EventArgs e)
        {
            product3 prd = new product3();
            prd.Check = 0; // 인덱서의 길이

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((Convert.ToBoolean(row.Cells[0].Value) == true)) // 0번째 체크 박스의 값이 있으면
                {
                    prd[prd.Check] = row.Cells[1].Value.ToString(); // prd 클래스의 인덱서에 상품명을 저장

                    prd.Check += 1; // 프로퍼티 값 +1
                }
            }


            // 비주얼 베이직 참조 추가 후 인풋 박스를 이용해 재고 수량 입력 후 string -> int 변환

            for (int i = 0; i < prd.size(); i++)

            {
                // 박스에 입력하는 수량을 텍스트로 받아옴
                if(MessageBox.Show(prd[i] + "을(를) 삭제 하시겠습니까?", "상품 삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (Pd.Pro_delete(prd[i], dataGridView1))
                    {
                        MessageBox.Show(prd[i] + "이(가) 삭제 되었습니다.");
                    }
                    else
                    {
                        MessageBox.Show(prd[i] + "는(은) 재고가 남아있습니다.");
                    }
                }

            }
        }

        private void product_del_FormClosing(object sender, FormClosingEventArgs e)
        {
            Pd.dbclose();
        }
    }
}
