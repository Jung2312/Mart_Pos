using System.Windows.Forms;
using System.Data.OleDb;
using System;
using System.Xml.Linq;
using static System.Windows.Forms.AxHost;
using System.Data;
using System.Security.Cryptography;
using System.Security.Policy;

namespace mart_pos
{
    // 추상클래스 생성
    abstract class dbabs
    {
        public abstract void Table_insert(DataGridView dg1); // 테이블 업데이트
        public abstract void dbclose(); // 디비 종료

    }

    // 판매 현황
    internal class SalesDB : dbabs 
    {
        // 디비 연결 구문
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=mart.accdb;Jet OLEDB:Database Password=qlwndjfwlsalswjd;");
        OleDbDataReader table = null; // 테이블 데이터를 읽어오는 변수
        OleDbCommand cmd = null;

        public SalesDB() {
            
            conn.Open(); // 디비 열기
        } 


        // 추상메소드
        public override void dbclose() // 디비 종료 메소드
        {   
            if(cmd != null) // cmd 를 사용 한 경우 종료
            {
                cmd.Dispose();
            }
            
            if(table != null) // table을 사용한 경우 종료
            {
                table.Close();
            }

            conn.Close(); // 무조건 종료
        }


        // 추상메소드
        // 판매 현황
        public override void Table_insert(DataGridView dg1) // 판매 현황 테이블 생성
        {
            dg1.Rows.Clear(); // 테이블 데이터 삭제
            try
            {
                // 테이블에 넣고자 하는 컬럼명만 가져옴
                string sql = "SELECT productName, productPrice, productCount, productPrice from product";
                
                cmd = new OleDbCommand(sql, conn);
                table = cmd.ExecuteReader(); // 디비 데이터 읽음

                while (table.Read())
                {
                    dg1.Rows.Add(table["productName"], table["productPrice"], table["productCount"], 
                        Convert.ToInt32(table["productPrice"]) * Convert.ToInt32(table["productCount"]));
                }
            }
            catch(System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        
    }


    // 잔액
    internal class BalanceDB : dbabs
    {
        // 디비 연결 구문
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=mart.accdb;Jet OLEDB:Database Password=qlwndjfwlsalswjd;");
        OleDbDataReader table = null; // 테이블 데이터를 읽어오는 변수
        OleDbCommand cmd = null;


        public BalanceDB()
        {

            conn.Open(); // mysql 열기
        }

        // 추상메소드
        public override void dbclose() // 디비 종료 메소드
        {
            if (cmd != null) // cmd 를 사용 한 경우 종료
            {
                cmd.Dispose();
            }

            if (table != null) // table을 사용한 경우 종료
            {
                table.Close();
            }

            conn.Close(); // 무조건 종료
        }

        // 추상메소드
        // 잔액 조회
        public override void Table_insert(DataGridView dg1) // 잔액 조회 테이블 생성
        {
            try
            {
                dg1.Rows.Clear();
                // 테이블에 넣고자 하는 컬럼명만 가져옴
                string sql = "SELECT today, existingAmount, reductionAmount, balance from price";

                cmd = new OleDbCommand(sql, conn);
                table = cmd.ExecuteReader();

                while (table.Read())
                {
                    if(table != null)
                    {
                        dg1.Rows.Add(table["today"], table["existingAmount"], table["reductionAmount"], table["balance"]);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void price_append(int ea, DataGridView dg1) // 잔액 추가 메소드
        {
            try
            {
                string sql = "select existingAmount, reductionAmount from price"; // 잔액 필드 검색
                cmd = new OleDbCommand(sql, conn);
                OleDbDataReader de = cmd.ExecuteReader();

                int price = 0; // 누적 금액 계산 변수

                while (de.Read())
                {
                    price += (Convert.ToInt16(de["existingAmount"]) - Convert.ToInt16(de["reductionAmount"])); // 누적 잔액 계산
                }
                cmd.Dispose(); // 조회 후 cmd 종료

                sql = "Insert into price (today, existingAmount,reductionAmount, balance) values (@today, @existingAmount,@reductionAmount, @balance)";
                cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@today", DateTime.Today); // 파라미터를 이용해 매개변수 대입(오늘날짜)
                cmd.Parameters.AddWithValue("@existingAmount", ea); // 파라미터를 이용해 매개변수 대입(기존금액)
                cmd.Parameters.AddWithValue("@reductionAmount", 0); // 파라미터를 이용해 매개변수 대입(감소금액)
                cmd.Parameters.AddWithValue("@balance", (price + ea)); // 파라미터를 이용해 매개변수 대입(잔액)
                cmd.ExecuteNonQuery(); // 데이터 전송
                cmd.Dispose();

                MessageBox.Show("금액이 추가되었습니다.");
                Table_insert(dg1); // 데이블 업데이트

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    }


    // 메인
    internal class MainDB : dbabs
    {
        // 디비 연결 구문
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=mart.accdb;Jet OLEDB:Database Password=qlwndjfwlsalswjd;");
        OleDbDataReader table = null; // 테이블 데이터를 읽어오는 변수
        OleDbCommand cmd = null;
        public MainDB()
        {

            conn.Open(); // 디비 열기
        }
        
        // 추상메소드
        public override void dbclose() // 디비 종료 메소드
        {

            if (cmd != null) // cmd 를 사용 한 경우 종료
            {
                cmd.Dispose();
            }

            if (table != null) // table을 사용한 경우 종료
            {

                table.Close();
            }

            conn.Close(); // 무조건 종료
        }


        public bool Check_quantity(string name, int qua) // 입력한 수량이 재고 수량을 넘기지 않는지 확인
        {
            try
            {
                string sql = "SELECT availableStock from product where productName = @productName and availableStock >= @availableStock";

                cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@productName", name); // 상품명
                cmd.Parameters.AddWithValue("@availableStock", qua); // 주문 수량
                table = cmd.ExecuteReader();

                while (table.Read())
                {
                    if (table != null) // 입력한 수량와 동일하거나 많은 수량이 있는경우
                    {
                        return true;
                    }

                    else
                    {
                        return false;
                    }
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }

            return false;

        }

        public void Main_data(string name, int won, int qua) // 메인 테이블에 데이터를 보내기 위해 디비 연결
        {
            try
            {
                string sql = "Insert into selecttable (selectName,selectPrice, selQuality, selectSum) values " +
                    "(@selectName, @selectPrice, @selQuality, @selectSum)";

                cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@selectName", name); // 파라미터를 이용해 매개변수 대입 상품명
                cmd.Parameters.AddWithValue("@selectPrice", won); // 파라미터를 이용해 매개변수 대입 상품 금액
                cmd.Parameters.AddWithValue("@selQuality", qua); // 파라미터를 이용해 매개변수 대입 주문 수량
                cmd.Parameters.AddWithValue("@selectSum", won * qua); // 파라미터를 이용해 매개변수 대입 결제 금액

                cmd.ExecuteNonQuery(); // 데이터 전송

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }

        }

        // 추상메소드
        // 메인 테이블 업데이트
        public override void Table_insert(DataGridView dg1) // 선택한 품목 조회 테이블 생성
        {
            try
            {
                dg1.Rows.Clear();
                // 테이블에 넣고자 하는 컬럼명만 가져옴
                string sql = "SELECT selectName, selectPrice, selQuality, selectSum, seldisCount from selecttable";
                // 이름 가격, 수량 
                int price = 0;
                cmd = new OleDbCommand(sql, conn);
                table = cmd.ExecuteReader();

                while (table.Read())
                {
                    dg1.Rows.Add(table["selectName"], table["selectPrice"], table["selQuality"], table["selectSum"], table["seldisCount"]);
                    price += Convert.ToInt32(table["selectSum"]);
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }

        }

        // 총 결제해야 하는 금액 확인
        public void price_sum(TextBox t1, DataGridView dg1)
        {
            try
            {
                int money = 0;
                for (int i = 0; i < dg1.Rows.Count; i++)
                {
                    money += Convert.ToInt32(dg1.Rows[i].Cells[2].Value)* Convert.ToInt32(dg1.Rows[i].Cells[3].Value);
                }

                // 텍스트 박스에 금액 추가
                t1.Text = money.ToString();

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message+ex.StackTrace);
            }
        }

        public void table_del() // 프로그램 종료시 메인 테이블 데이터 삭제
        {
            try
            {
                string sql = "DELETE FROM selecttable";
                cmd = new OleDbCommand(sql, conn);
                cmd.ExecuteNonQuery(); // 데이터 전송

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
        

        // 할인을 적용하기 위한 회원 검색
        public bool user_search(string phone) 
        {
            try
            {
                string sql = "SELECT userPhone from usertable where userPhone = @userPhone";

                cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userPhone", phone);
                table = cmd.ExecuteReader();

                while (table.Read())
                {
                    if (table != null) // 해당 전화번호를 가진 회원이 존재하는 경우
                    {
                        return true;
                    }

                    else
                    {
                        return false;
                    }
                }
                    
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }

            return false;
        }
        
        // 회원 할인
        public string user_discount(string phone)
        {
            try
            {
                string sql = "SELECT userPrice from usertable where userPhone = @userPhone";

                cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userPhone", phone);
                table = cmd.ExecuteReader();

                while (table.Read())
                {
                    if (table != null)
                    {
                        // 회원의 전화번호 전송
                        string sql2 = "UPDATE selecttable SET userPhone = @userPhone";
                        OleDbCommand cmd2 = new OleDbCommand(sql2, conn);
                        cmd2.Parameters.AddWithValue("@userPhone", phone); // 파라미터를 이용해 매개변수 대입

                        cmd2.ExecuteNonQuery(); // 데이터 전송
                        cmd2.Dispose();

                        if (Convert.ToInt32(table["userPrice"]) >= 15000
                        && Convert.ToInt32(table["userPrice"]) < 30000)
                        {
                            return "RED";
                        }

                        else if (Convert.ToInt32(table["userPrice"]) >= 30000
                            && Convert.ToInt32(table["userPrice"]) < 50000)
                        {
                            return "GREEN";
                        }

                        else if (Convert.ToInt32(table["userPrice"]) >= 50000)
                        {
                            return "BLUE";
                        }
                    }
                    

                }


            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }

            return null; // 아무 등급이 아닌 경우 null 반환
        }

        // 결제 후 재고 감소, 판매 개수 증가
        public void payment_data(string name, int sales)
        {
            try
            {
                // 결제한 만큼 재고 줄임
                string sql = "UPDATE product SET availableStock = availableStock - @availableStock WHERE productName = @productName";
                cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@availableStock", sales); // 파라미터를 이용해 매개변수 대입
                cmd.Parameters.AddWithValue("@productName", name); // 파라미터를 이용해 매개변수 대입
                cmd.ExecuteNonQuery(); // 데이터 전송

                cmd.Dispose();

                // 결제 한 만큼 판매 늘림
                string sql2 = "UPDATE product SET productCount = productCount + @productCount WHERE productName = @productName";
                cmd = new OleDbCommand(sql2, conn);
                cmd.Parameters.AddWithValue("@productCount", sales); // 파라미터를 이용해 매개변수 대입
                cmd.Parameters.AddWithValue("@productName", name); // 파라미터를 이용해 매개변수 대입
                cmd.ExecuteNonQuery(); // 데이터 전송

                cmd.Dispose();

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
        
        // 받은 금액과 거스름돈을 잔액 테이블에 추가(회원 누적 금액도 추가)
        public bool balance_data(int won, int won2)
        {
            try
            {
                // 거스름돈 잔액에서 빼기
                string sql = "select existingAmount, reductionAmount from price"; // 잔액 필드 검색
                cmd = new OleDbCommand(sql, conn);
                OleDbDataReader de = cmd.ExecuteReader();
                int price = 0;
                while (de.Read())
                {
                    if(de != null)
                    {
                        price += (Convert.ToInt16(de["existingAmount"]) - Convert.ToInt16(de["reductionAmount"])); // 누적 잔액 계산
                    }
                }
                cmd.Dispose();

                if(price >= won2) // 포스기의 잔액이 거스름돈 보다 더 많은 경우에만 결제 가능
                {
                    if (won2 == 0) //  거스름돈이 없는 경우 잔액에 추가
                    {
                        string sql2 = "Insert into price (today, existingAmount, reductionAmount, balance) values (@today, @existingAmount,@reductionAmount, @balance)";
                        cmd = new OleDbCommand(sql2, conn);
                        cmd.Parameters.AddWithValue("@today", DateTime.Today); // 파라미터를 이용해 매개변수 대입
                        cmd.Parameters.AddWithValue("@existingAmount", won); // 파라미터를 이용해 매개변수 대입
                        cmd.Parameters.AddWithValue("@reductionAmount", 0); // 파라미터를 이용해 매개변수 대입
                        cmd.Parameters.AddWithValue("@balance", (price + won)); // 파라미터를 이용해 매개변수 대입 
                        cmd.ExecuteNonQuery(); // 데이터 전송
                        cmd.Dispose();
                    }

                    else // 거스름돈이 있는 경우 잔액에서 해당 금액 빼기
                    {
                        string sql2 = "Insert into price (today, existingAmount, reductionAmount, balance) values (@today, @existingAmount,@reductionAmount, @balance)";
                        cmd = new OleDbCommand(sql2, conn);
                        cmd.Parameters.AddWithValue("@today", DateTime.Today); // 파라미터를 이용해 매개변수 대입
                        cmd.Parameters.AddWithValue("@existingAmount", won); // 파라미터를 이용해 매개변수 대입
                        cmd.Parameters.AddWithValue("@reductionAmount", won2); // 파라미터를 이용해 매개변수 대입
                        cmd.Parameters.AddWithValue("@balance", (price - won2) + won); // 파라미터를 이용해 매개변수 대입 
                        cmd.ExecuteNonQuery(); // 데이터 전송
                        cmd.Dispose();
                    }

                    // 결제 한 만큼 회원 누적 금액 늘림
                    string sql3 = "UPDATE usertable SET userPrice = userPrice + @userPrice WHERE userPhone IN (SELECT userPhone from selecttable)";
                    cmd = new OleDbCommand(sql3, conn);
                    cmd.Parameters.AddWithValue("@userPrice", won - won2); // 파라미터를 이용해 매개변수 대입(실제 받은 금액만)

                    cmd.ExecuteNonQuery(); // 데이터 전송 
                    cmd.Dispose();
                    return true;
                }

                return false;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
                return false;
            }
        }
    }
     
    
    // 상품
    internal class ProductDB : dbabs
    {
        // 디비 연결 구문
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=mart.accdb;Jet OLEDB:Database Password=qlwndjfwlsalswjd;");
        OleDbDataReader table = null; // 테이블 데이터를 읽어오는 변수
        OleDbCommand cmd = null;


        public ProductDB()
        {

            conn.Open(); // 디비 열기
        }

        // 추상메소드
        public override void dbclose() // 디비 종료 메소드
        {
            if (cmd != null) // cmd 를 사용 한 경우 종료
            {
                cmd.Dispose();
            }

            if (table != null) // table을 사용한 경우 종료
            {
                table.Close();
            }

            conn.Close(); // 무조건 종료
        }


        public void del_Table(DataGridView dg1) // 상품삭제 테이블 생성
        {
            try
            {
                dg1.Rows.Clear();
                // 테이블에 넣고자 하는 컬럼명만 가져옴
                string sql = "SELECT productName, productPrice, availableStock from product";

                cmd = new OleDbCommand(sql, conn);
                table = cmd.ExecuteReader();

                while (table.Read())
                {
                    dg1.Rows.Add(false, table["productName"], table["productPrice"], table["availableStock"]);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        // 추상 메소드
        // 상품 선택
        public override void Table_insert(DataGridView dg1) // 상품 선택 테이블 생성
        {
            try
            {
                dg1.Rows.Clear();
                // 테이블에 넣고자 하는 컬럼명만 가져옴
                string sql = "SELECT productName, productPrice from product where availableStock > 0";

                cmd = new OleDbCommand(sql, conn);
                table = cmd.ExecuteReader();

                while (table.Read())
                {
                    dg1.Rows.Add(false, table["productName"], table["productPrice"]);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        // 상품 등록
        public bool Pro_insert(string name, string price)
        {
            try
            {
                string sql = "select productName from product where productName LIKE @productName";
                cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@productName", name); // 파라미터를 이용해 매개변수 대입
                table = cmd.ExecuteReader();
                int i = 0;

                while (table.Read())
                {
                    i += 1;
                }


                if (i == 0) // 해당하는 이름을 가진 상품이 없다면 등록
                {
                    table.Close();
                    cmd.Dispose();

                    sql = "Insert into product (productName, productPrice) values (@productName, @productPrice)";
                    cmd = new OleDbCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@productName", name); // 파라미터를 이용해 매개변수 대입
                    cmd.Parameters.AddWithValue("@productPrice", price); // 파라미터를 이용해 매개변수 대입
                    cmd.ExecuteNonQuery(); // 데이터 전송

                    return true;
                }

                else
                {
                    return false;
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        // 상품 삭제
        public bool Pro_delete(string name, DataGridView dg1) // 상품 삭제 메소드
        {

            try
            {
                // 재고수량이 0인 상품
                string sql = "select productName from product where productName = @productName and availableStock = 0";
                cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@productName", name); // 파라미터를 이용해 매개변수 대입
                table = cmd.ExecuteReader();
                int i = 0;
                while (table.Read())
                {
                    i += 1;
                }

                if (i != 0) // 해당하는 이름을 가진 상품이면서 재고가 0인 경우 삭제
                {
                    table.Close();
                    cmd.Dispose();

                    // 재고수량이 0인 경우만 삭제 가능하도록
                    sql = "delete from product where productName = @productName and availableStock = 0";
                    cmd = new OleDbCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@productName", name); // 파라미터를 이용해 매개변수 대입
                    cmd.ExecuteNonQuery(); // 데이터 전송

                    MessageBox.Show("상품이 삭제되었습니다.");
                    del_Table(dg1); // 테이블 업데이트
                    return true;
                }

                else // 재고가 남은 경우 삭제 불가능
                {
                    return false;
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;

        }

    }

    internal class StockDB : dbabs // 재고 조회
    {

        // 디비 연결 구문
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=mart.accdb;Jet OLEDB:Database Password=qlwndjfwlsalswjd;");
        OleDbDataReader table = null; // 테이블 데이터를 읽어오는 변수
        OleDbCommand cmd = null;


        public StockDB()
        {

            conn.Open(); // 디비 열기
        }

        // 추상메소드
        public override void dbclose() // 디비 종료 메소드
        {
            if (cmd != null) // cmd 를 사용 한 경우 종료
            {
                cmd.Dispose();
            }

            if (table != null) // table을 사용한 경우 종료
            {
                table.Close();
            }

            conn.Close(); // 무조건 종료
        }

        // 추상메소드
        // 재고 현황
        public override void Table_insert(DataGridView dg1) // 재고 현황 테이블 생성
        {
            try
            {
                dg1.Rows.Clear();
                // 테이블에 넣고자 하는 컬럼명만 가져옴
                string sql = "SELECT productName, productPrice, availableStock from product";

                cmd = new OleDbCommand(sql, conn);
                table = cmd.ExecuteReader();

                while (table.Read())
                {
                    dg1.Rows.Add(false, table["productName"], table["productPrice"], table["availableStock"]);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void pro_append(string text, int num, DataGridView dg1) // 재고 주문 메소드
        {
            try
            {
                // 수정하고자 넣고자 하는 컬럼명만 가져옴
                string sql = "UPDATE product SET availableStock = availableStock + @availableStock " +
                    "WHERE productName = @productName";


                cmd = new OleDbCommand(sql, conn);

                cmd.Parameters.AddWithValue("@availableStock", num); // 파라미터를 이용해 매개변수 대입 
                cmd.Parameters.AddWithValue("@productName", text); // 파라미터를 이용해 매개변수 대입 
                cmd.ExecuteNonQuery(); // 데이터 전송

                MessageBox.Show("주문이 완료되었습니다.");
                Table_insert(dg1); // 데이블 업데이트

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }

    // 회원 
    internal class UserDB : dbabs
    {
        // 디비 연결 구문
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=mart.accdb;Jet OLEDB:Database Password=qlwndjfwlsalswjd;");
        OleDbDataReader table = null; // 테이블 데이터를 읽어오는 변수
        OleDbCommand cmd = null;
        

        public UserDB() {
            
            conn.Open(); // 디비 열기
        }

        // 추상메소드
        public override void dbclose() // 디비 종료 메소드
        {   
            if(cmd != null) // cmd 를 사용 한 경우 종료
            {
                cmd.Dispose();
            }
            
            if(table != null) // table을 사용한 경우 종료
            {
                table.Close();
            }

            conn.Close(); // 무조건 종료
        }


        // 추상메소드
        // 회원 관리
        public override void Table_insert(DataGridView dg1) // 회원 조회 테이블 생성
        {
            dg1.Rows.Clear();
            try
            {
                // 테이블에 넣고자 하는 컬럼명만 가져옴
                string sql = "SELECT userName, userPhone, userPrice FROM usertable";

                cmd = new OleDbCommand(sql, conn);
                table = cmd.ExecuteReader();


                while (table.Read())
                {

                    dg1.Rows.Add(false, table["userName"], table["userPhone"], table["userPrice"]);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void User_search(string phone, DataGridView dg1) // 회원 검색
        {
            dg1.Rows.Clear();
            try
            {
                // 테이블에 넣고자 하는 컬럼명만 가져옴
                string sql = "SELECT userName, userPhone, userPrice FROM usertable WHERE userPhone LIKE '%' + @userPhone + '%'";

                cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userPhone", phone); // 파라미터를 이용해 매개변수 대입
                table = cmd.ExecuteReader();


                while (table.Read())
                {

                    dg1.Rows.Add(false, table["userName"], table["userPhone"], table["userPrice"]);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void User_delete(string num, DataGridView dg1) // 회원 삭제 메소드
        {
            try
            {
                string sql = "delete from usertable where userPhone = @userPhone";
                cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userPhone", num); // 파라미터를 이용해 매개변수 대입
                cmd.ExecuteNonQuery(); // 데이터 전송

                MessageBox.Show("회원이 삭제되었습니다.");
                Table_insert(dg1); // 데이블 업데이트

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public bool User_insert(string name, string phone) // 회원 등록 메소드
        {
            try
            {

                // 해당하는 전화번호가 있을 경우 등록 불가능
                string sql = "select userPhone from usertable where userPhone LIKE @userPhone";
                cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userPhone", phone); // 파라미터를 이용해 매개변수 대입
                table = cmd.ExecuteReader();
                int i = 0;
                while (table.Read())
                {
                    i += 1;
                }
                if (i == 0) // 해당 전화번호가 없는 경우
                {
                    table.Close();
                    cmd.Dispose();
                    sql = "Insert into usertable (userName, userPhone) values (@userName, @userPhone)";
                    cmd = new OleDbCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userName", name); // 파라미터를 이용해 매개변수 대입
                    cmd.Parameters.AddWithValue("@userPhone", phone); // 파라미터를 이용해 매개변수 대입
                    cmd.ExecuteNonQuery(); // 데이터 전송

                    return true;
                }

                else
                {
                    return false;
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }


    }

    
}
