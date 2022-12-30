using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace mart_pos
{
    public partial class product_select : Form
    {
        ProductDB Pd = new ProductDB();
        MainDB md = new MainDB();
        Form1 frm1; // 메인 폼 불러옴

        public product_select()
        {
            InitializeComponent();
            Pd.Table_insert(dataGridView1);
        }
        
        public product_select(Form1 f1) // 메인폼을 포함하고 있는 생성자 생성(테이블 업데이트용)
        {
            InitializeComponent();
            Pd.Table_insert(dataGridView1);
            frm1 = f1;
        }

        private void button2_Click(object sender, EventArgs e) // 뒤로 가기 버튼
        {
            this.Close(); // 현재 창 닫기
            Pd.dbclose(); // 디비 종료
        }

        private void button1_Click(object sender, EventArgs e) // 추가 버튼
        {
            product2 prd = new product2();
            select sel = new select();
            
            prd.Check =  0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((Convert.ToBoolean(row.Cells[0].Value) == true)) // 0번째 체크 박스의 값이 있으면
                {
                    prd[prd.Check] = row.Cells[1].Value.ToString(); // prd 클래스의 인덱서에 이름 저장
                    sel[prd.Check] = Convert.ToInt32(row.Cells[2].Value); // sel 클래스의 인덱서에 단가 저장
                    prd.Check += 1; // 프로퍼티 값 +1(인덱서 사이즈 체크용)
                }
            }

            // 비주얼 베이직 참조 추가 후 인풋 박스를 이용해 재고 수량 입력 후 string -> int 변환

            for (int i = 0; i < prd.size(); i++)

            {
                // 박스에 입력하는 수량을 텍스트로 받아옴
                string input = Microsoft.VisualBasic.Interaction.InputBox(prd[i] + "의 수량을 입력하세요.", "상품 선택", "상품 수량을 입력하세요.");

                if (input != null) // 문자열이 공백이 아닌 경우
                {
                    if (int.TryParse(input, out int n)) // 숫자외 수량을 입력했을 경우
                    {
                        if (md.Check_quantity(prd[i], n)) // 재고 수량보다 적은 금액을 입력한 경우만 선택 가능
                        {
                            // 이름과 수량을 추가 후 디비에서 기존 데이터를 비교해 테이블에 다른 값 출력
                            sel.Qua = n;
                            md.Main_data(prd[i], sel[i], sel.Qua); // 선택한 데이터를 디비로 전송
                            
                        }
                        else
                        {
                            MessageBox.Show("재고가 부족합니다. 재고를 추가해주세요");
                        }
                        
                    }
                }

            }

            if (MessageBox.Show("회원 할인을 활성화하시겠습니까?", "회원 할인", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                frm1.button25.Enabled = true; // 결제 완료시 영수증 출력
                MessageBox.Show("회원 할인 버튼을 눌러주세요.");
            }

            md.Table_insert(frm1.dataGridView1); // 메인 폼의 테이블 업데이트
            md.price_sum(frm1.textBox3, frm1.dataGridView1); // 결제 금액 확인
            this.Close();
            md.dbclose();

        }

        private void product_select_FormClosing(object sender, FormClosingEventArgs e)
        {
            md.dbclose();
        }

        private void product_select_Load(object sender, EventArgs e)
        {

        }
    }
}