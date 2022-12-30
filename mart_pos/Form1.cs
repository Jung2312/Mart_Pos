using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Text;
using System.IO;
using System.Resources;
using mart_pos.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices.ComTypes;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Asn1.IsisMtt;

namespace mart_pos
{
    
    public partial class Form1 : Form
    {
        PrivateFontCollection privateFonts = new PrivateFontCollection(); // 실행파일에 폰트 적용

        MainDB md = new MainDB(); // 디비 연결
        int discount = 0;
        public int Price { get; set; } // 문자로 받은 금액 정수형으로 변환
        public string Sprice { get; set; } // 받은 금액

        public Form1()
        {
            InitializeComponent();
            md.table_del(); // 디비 데이터 삭제
        }

        private void button6_Click(object sender, EventArgs e) // 상품 선택
        {
            product_select ps = new product_select(this); // 상품 선택 객체가 생성되면서 메인 폼 포함
            ps.ShowDialog();
        }

    private void button7_Click(object sender, EventArgs e)  // 상품 등록
        {
            product_registration pr = new product_registration();
            pr.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e) // 판매현황
        {
            sales_table st = new sales_table();
            st.ShowDialog();

            
        }

        private void button2_Click(object sender, EventArgs e) // 재고 현황
        {
            product_table pt = new product_table();
            pt.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e) // 회원 등록
        {
            user_registration ur = new user_registration();
            ur.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e) // 회원 관리
        {
            user_table ut = new user_table();
            ut.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e) // 잔액 확인
        {
            Balance_Inquiry bi = new Balance_Inquiry();
            bi.ShowDialog();
        }

       
        private void button9_Click(object sender, EventArgs e) // 상품 삭제
        {
            product_del del = new product_del();
            del.ShowDialog();
        }


        // 결제 버튼
        private void button8_Click(object sender, EventArgs e)
        {

            // 테이블에 데이터가 있는 경우만 결제 되게
            if(dataGridView1.Rows.Count != 0)
            {
               
                if (textBox6.Text == "") // 공백만 들어있는 경우
                {
                    MessageBox.Show("받은 금액이 없습니다.");
                }

                else
                {
                    md.price_sum(textBox3, dataGridView1); // 결제 금액 확인

                    Price = int.Parse(Sprice); // 계산기에서 들어온 문자를 int로 형변환

                    if (Price > 0)
                    {
                        if (Price >= (int.Parse(textBox3.Text))) // 받은 금액이 실 결제 금액보다 큰 경우
                        {

                            //// 테이블에 있는 금액 합산 후 받은 금액 - 합산 금액
                            md.price_sum(textBox3, dataGridView1); // 결제 금액 확인
                            textBox4.Text = Price.ToString(); // 받은 금액
                            textBox5.Text = (Price - Convert.ToInt32(textBox3.Text)).ToString(); // 거스름돈

                            // 디비에 잔액 추가/ 감소
                            if (md.balance_data(Convert.ToInt32(textBox4.Text), Convert.ToInt32(textBox5.Text)))
                            {
                                MessageBox.Show("결제 되었습니다.");

                                // 디비에 재고 데이터 감소, 판매 데이터 추가
                                for (int i = 0; i < dataGridView1.RowCount; i++)
                                {
                                    md.payment_data(dataGridView1.Rows[i].Cells[0].Value.ToString(),
                                        Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value));
                                }


                                if (MessageBox.Show("영수증을 출력하시겠습니까?", "영수증 출력", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                                {
                                    button22.Enabled = true; // 결제 완료시 영수증 출력버튼 활성화
                                    MessageBox.Show("출력 버튼을 눌러주세요.");
                                }
                                else // 영수증을 출력 안하는 경우 데이터 지우기
                                {
                                    textBox3.Text = "";
                                    textBox4.Text = "";
                                    textBox5.Text = "";
                                    textBox6.Text = "";
                                    // 누적 금액 초기화

                                    button25.Enabled = false; // 결제 완료되면 바로 할인 버튼 잠금

                                    md.table_del(); // 디비에 존재하는 데이터 삭제
                                    md.Table_insert(dataGridView1); // 테이블 업데이트
                                }
                            }

                            else // 현재 남은 작앤 보다 큰 금액을 받은 경우
                            {
                                MessageBox.Show("잔액이 부족합니다.");
                                textBox3.Text = "";
                                textBox4.Text = "";
                                textBox5.Text = "";
                                textBox6.Text = "";
                                // 누적 금액 초기화

                            }

                        }

                        else // 받은 금액이 결제 금액 보다 적은 경우
                        {
                            MessageBox.Show("금액이 부족합니다.");
                            // 누적 금액 초기화
                            textBox6.Text = "";
                        }
                    }

                    else // 받은 금액이 결제 금액 보다 적은 경우
                    {
                        MessageBox.Show("금액이 부족합니다.");
                        // 누적 금액 초기화
                        textBox6.Text = "";
                    }

                }

            }
            else
            {
                MessageBox.Show("상품을 선택하세요");
                textBox6.Text = "";
            }

        }

        // 계산기 숫자 버튼
       
        private void button10_Click(object sender, EventArgs e) // 7
        {
            textBox6.Text += button10.Text.ToString();
            Sprice = textBox6.Text;
            
        }

        private void button11_Click(object sender, EventArgs e) // 8
        {
            textBox6.Text += button11.Text.ToString();
            Sprice = textBox6.Text;
        }

        private void button12_Click(object sender, EventArgs e) // 9
        {
            textBox6.Text += button12.Text.ToString();
            Sprice = textBox6.Text;
        }

        private void button15_Click(object sender, EventArgs e) // 4
        {
            textBox6.Text += button15.Text.ToString();
            Sprice = textBox6.Text;
        }

        private void button14_Click(object sender, EventArgs e) // 5
        {
            textBox6.Text += button14.Text.ToString();
            Sprice = textBox6.Text;
        }

        private void button13_Click(object sender, EventArgs e) // 6
        {
            textBox6.Text += button13.Text.ToString();
            Sprice = textBox6.Text;
        }

        private void button18_Click(object sender, EventArgs e) // 1
        {
            textBox6.Text += button18.Text.ToString();
            Sprice = textBox6.Text;
        }

        private void button17_Click(object sender, EventArgs e) // 2
        {
            textBox6.Text += button17.Text.ToString();
            Sprice = textBox6.Text;
        }

        private void button16_Click(object sender, EventArgs e) // 3
        {
            textBox6.Text += button16.Text.ToString();
            Sprice = textBox6.Text;
        }

        private void button21_Click(object sender, EventArgs e) // 0
        {
            textBox6.Text += button21.Text.ToString();
            Sprice = textBox6.Text;
        }

        private void button20_Click(object sender, EventArgs e) // 00
        {
            textBox6.Text += button20.Text.ToString();
            Sprice = textBox6.Text;

        }

        // 삭제 버튼(C)
        private void button19_Click(object sender, EventArgs e)
        {
            textBox6.Text = null;
            Sprice = null;
        }


        // 한칸 지우기 버튼(>)
        private void button23_Click(object sender, EventArgs e)
        {
            if(textBox6.Text.Length != 0) // 텍스트박스가 공백이 아닌 경우
            {
                textBox6.Text = textBox6.Text.Substring(0, textBox6.Text.Length - 1); // 마지막 위치의 문자를 지움
                Sprice = textBox6.Text;
            }
            
        }


        // 영수증 출력
        private void button22_Click(object sender, EventArgs e)
        {
            
            PrintDialog PrintDialog1 = new PrintDialog();
            
            PrintDialog1.AllowSomePages = true; // 페이지 옵션 단추
            
            PrintDialog1.ShowHelp = true; // 도움말 단추
            
            DialogResult result = PrintDialog1.ShowDialog(); // 프린트 대화 상자

            // 확인을 누르면 프린트
            if (result == DialogResult.OK)
            {
                SaveFileDialog pdfSaveDialog = new SaveFileDialog();
                printDocument1.DefaultPageSettings.PaperSize = new PaperSize("영수증", 400, 900); // 페이지 사이즈
                printDocument1.Print();
            }

            
        }

        // 프린트 페이지 설정
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            privateFonts.AddFontFile("NANUMMYEONGJOECO.TTF"); // 폰트 불러옴

            Font printFont = new Font(privateFonts.Families[0], 24); // 폰트
            Font printFont2 = new Font(privateFonts.Families[0], 12);

            int num = 0; // 금액
            int cnt = 0; // 위치

            string t1 = null; //상품명
            string t2 = null; // 수량

            // 영수증 기본 내용 출력
            e.Graphics.DrawString("영 수 증", printFont, Brushes.Blue, 130, 80);
            e.Graphics.DrawString("상         호 : 동의마트", printFont2, Brushes.Blue, 60, 160);
            e.Graphics.DrawString("주         소 : 부산광역시 엄광로 176", printFont2, Brushes.Blue, 60, 180);
            e.Graphics.DrawString("전 화 번 호 : 051-890-0000", printFont2, Brushes.Blue, 60, 200);
            e.Graphics.DrawString("========================", printFont2, Brushes.Blue,60, 220);
            e.Graphics.DrawString("이   름            개   수      가    격", printFont2, Brushes.Blue, 60, 240);
            e.Graphics.DrawString("------------------------", printFont2, Brushes.Blue, 60, 260);

            
            try
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    t1 = dataGridView1.Rows[i].Cells[0].Value.ToString(); // 상품명
                    t2 = dataGridView1.Rows[i].Cells[2].Value.ToString(); // 수량
                    num = (Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value) * 
                        Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value)); // 개수 * 금액
                    e.Graphics.DrawString(String.Format("{0}{1,13}{2,18}", t1, t2, num.ToString()), 
                        printFont2, Brushes.Blue, 60, 280 + cnt);

                    cnt += 20; // 반복을 할때마다 위치 변경
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            e.Graphics.DrawString("------------------------", printFont2, Brushes.Blue, 60, cnt + 280);
            e.Graphics.DrawString("금         액 :                    "+ textBox3.Text, printFont2, Brushes.Blue, 60, cnt + 300);
            e.Graphics.DrawImage(Properties.Resources.blu2, 100, cnt + 360); // 바코드 이미지 출력
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        // 폼 닫으면서 디비, 테이블 종료
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            md.table_del();
            md.dbclose();
        }

        // 회원 할인 적용
        private void button25_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("회원의 전화번호를 입력하세요", "상품 선택", "010");
            
            if (input != null) // 문자열이 공백이 아닌 경우
            {
                if(input.Length == 11)
                {
                    // 디비에서 해당하는 전화번호를 가진 회원인지 확인
                    if(md.user_search(input))
                    {
                        if(md.user_discount(input) == "RED") // 회원 등급 확인
                        {
                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                // 할인 금액 계산
                                discount = (int)(Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value) * 0.01); // 금액에 할인율 적용
                                dataGridView1.Rows[i].Cells[3].Value = (int)(Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value) - Convert.ToInt32(discount)); // 할인된 금액 표시
                                dataGridView1.Rows[i].Cells[4].Value = Convert.ToInt32(discount); // 실 할인 금액
                            }
                            dataGridView1.Update(); // 테이블 업데이트
                            button25.Enabled = false; // 버튼 비활성화
                            md.price_sum(textBox3, dataGridView1); // 결제 금액 확인
                        }
                        
                        else if(md.user_discount(input) == "GREEN")
                        {
                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                // 할인 금액 계산
                                discount = (int)(Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value) * 0.02);
                                dataGridView1.Rows[i].Cells[3].Value = (int)(Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value) - Convert.ToInt32(discount));
                                dataGridView1.Rows[i].Cells[4].Value = Convert.ToInt32(discount);
                            }

                            dataGridView1.Update(); // 테이블 업데이트
                            button25.Enabled = false; // 버튼 비활성화
                            md.price_sum(textBox3, dataGridView1); // 결제 금액 확인
                        }
                        
                        else if(md.user_discount(input) == "BLUE")
                        {
                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                // 할인 금액 계산
                                discount = (int)(Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value) * 0.03);
                                dataGridView1.Rows[i].Cells[3].Value = (int)(Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value) - Convert.ToInt32(discount));
                                dataGridView1.Rows[i].Cells[4].Value = Convert.ToInt32(discount);
                            }

                            dataGridView1.Update(); // 테이블 업데이트
                            button25.Enabled = false; // 버튼 비활성화
                            md.price_sum(textBox3, dataGridView1); // 결제 금액 확인
                        }

                        else // 등급이 낮은 회원
                        {
                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                // 할인 금액 없음
                                discount = 0;
                            }

                            button25.Enabled = false; // 버튼 비활성화
                            md.price_sum(textBox3, dataGridView1); // 결제 금액 확인
                        }

                    }

                    else
                    {
                        MessageBox.Show("존재하지 않는 회원입니다.");

                    }
                }

                else
                {
                    MessageBox.Show("전화번호는 11자리로 입력하세요.\nex) 01000000000");
                }

            }
        }

        // 프린트가 완료 되면 데이터 없앰
        private void printDocument1_EndPrint(object sender, PrintEventArgs e)
        {
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";

            md.table_del(); // 디비 데이터 삭제
            md.Table_insert(dataGridView1); // 테이블 데이터 업데이트

            button22.Enabled = false; // 버튼 잠금
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
