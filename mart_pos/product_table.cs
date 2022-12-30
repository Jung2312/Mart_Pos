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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace mart_pos
{
    public partial class product_table : Form
    {

        StockDB Sd = new StockDB(); // 데이터 베이스를 사용하기 위한 클래스 생성


        public product_table()
        {
            InitializeComponent();
            Sd.Table_insert(dataGridView1); // 테이블 채우는 메소드 사용
        }

        private void button2_Click(object sender, EventArgs e) // 뒤로 가기 버튼
        {
            this.Close(); // 현재 창 닫기
            Sd.dbclose(); // 디비 종료
        }

        private void button1_Click(object sender, EventArgs e) // 재고 주문 버튼
        {
            product prd = new product();
            prd.Check = 0; // 초기 길이 지정


            // 데이터 그리드 뷰의 전체 행 읽어옴
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((Convert.ToBoolean(row.Cells[0].Value) == true)) // 0번째 체크 박스의 값이 있으면
                {
                    prd[prd.Check] = row.Cells[1].Value.ToString(); // prd 클래스의 인덱서에 값을 저장

                    prd.Check += 1; // 프로퍼티 값 +1
                }
            }


            for (int i = 0; i < prd.size(); i++)
            {
                // 비주얼 베이직 참조 추가 후 인풋 박스를 이용해 재고 수량 입력 후 string -> int 변환

                // 박스에 입력하는 수량을 텍스트로 받아옴
                string input = Microsoft.VisualBasic.Interaction.InputBox(prd[i] + "을(를) 주문하시겠습니까?", "재고주문", "주문 수량을 입력하세요.");

                if (input != null) // 문자열이 공백이 아닌 경우
                {
                    if (int.TryParse(input, out int n)) // 숫자외 수량을 입력했을 경우
                    {
                        Sd.pro_append(prd[i], int.Parse(input), dataGridView1); // 디비의 재고 수량을 추가하는 메소드 사용
                    }
                    else
                    {
                        MessageBox.Show("숫자를 입력하세요.");
                    }
                }
            }

           }

        private void product_table_FormClosing(object sender, FormClosingEventArgs e)
        {
            Sd.dbclose();
        }
    }
    }
