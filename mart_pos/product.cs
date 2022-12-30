using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mart_pos
{
    
    // 재고 추가 상품명
    internal class product
    {
        public int Check { get; set; } // 인덱서의 길이 확인

        private string[] food = new string[100]; // 상품명을 가져오기 위한 인덱서
        public string this[int index]
        {
            get { return food[index]; }
            set { food[index] = value; }
        }

        public int size()
        {
            return Check; // 인덱서의 길이를 리턴
        }
    }
    
    // 상품 선택 상품명
    internal class product2 : product
    {
        public new int Check { get; set; } // 인덱서의 길이를 확인

        private string[] food = new string[100]; // 선택한 상품명
        public new string this[int index]
        {
            get { return food[index]; }
            set { food[index] = value; }
        }


        public new int size()
        {
            return Check; // 인덱서의 길이를 리턴
        }
    }
    
    // 삭제할 상품명
    internal class product3 : product2
    {
        public new int Check { get; set; } // 인덱서의 길이 확인 프로퍼티

        private string[] food = new string[100]; // 삭제할 상품 명 저장 인덱서
        public new string this[int index]
        {
            get { return food[index]; }
            set { food[index] = value; }
        }

        public new int size()
        {
            return Check; // 인덱서의 길이를 리턴
        }
    }

    // 상품 선택 상품단가
    internal class select : product
    {
        public int Qua { get; set; } // 주문수량
        public new int Check { get; set; } // 인덱서 길이

        private int[] Price = new int[100]; // 주문 금액
        public new int this[int index]
        {
            get { return Price[index]; }
            set { Price[index] = value; }
        }


        public new int size()
        {
            return Check; // 인덱서의 길이를 리턴
        }
    }

    // 회원의 전화번호를 받음
    internal class user 
    {
        string[] box = new string[100]; // 데이터 그리드에 들어오는 값을 확인하기 위한 배열 생성
        public int Check { get; set; }

        
        public string this[int index]
        {
            get { return box[index]; }
            set
            {
                box[index] = value;
            }
        }

        public  int size()
        {
            return Check; // 인덱서의 길이를 리턴
        }
    }


    // 회원의 이름 받음
    internal class user2 : user
    {
        string[] box = new string[100]; // 데이터 그리드에 들어오는 값을 확인하기 위한 배열 생성
        public new int Check { get; set; }

        
        public new string this[int index]
        {
            get { return box[index]; }
            set
            {
                box[index] = value;
            }
        }

        public new int size()
        {
            return Check; // 인덱서의 길이를 리턴
        }
    }

    
}
