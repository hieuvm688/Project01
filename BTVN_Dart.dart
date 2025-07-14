// 1/
// Viết chương trình nhận vào tuổi và điểm của một sinh viên, xác định nếu:

// Tuổi ≥ 18 và điểm ≥ 5 → "Đủ điều kiện"

// Ngược lại → "Không đủ điều kiện"

// 2/
// Cho biến String? username;, viết chương trình:

// Gán "Guest" nếu username đang null.

// Dùng toán tử ba ngôi để in:

// Nếu tên là "Admin" → in "Chào quản trị viên"

// Ngược lại → "Chào $username"
import 'dart:io';

void main()
{
  print("Nhập vào tuổi: ");
  int age = int.parse(stdin.readLineSync()!);
  print ("Nhập vào điểm: ");
  double score = double.parse(stdin.readLineSync()!);
  if (age >= 18 && score >= 5) {
    print("Đủ điều kiện");
  } else {
    print("Không đủ điều kiện");
  }
  print("Nhập vào tên người dùng: ");
  String? username = stdin.readLineSync();
  username ??= "Guest";
  String checkuser = (username == "Admin") ? "Chào quản trị viên" : "Chào $username";
  print(checkuser);
  
  print("Kết thúc chương trình");
  exit(0);
}