using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace WpfApp1
{
    public class DataManagement
    {
        public MySqlConnection conn;

        public DataManagement(MySqlConnection conn)
        {
            this.conn = conn;
        }

        #region 공통 함수
        public int NewIndexNum(String table)
        {
            try
            {
                conn.Open();
                var sql = "SELECT indexNum FROM " + table + " ORDER BY indexNum DESC LIMIT 1;";
                var mySqlCommand = new MySqlCommand(sql, conn);

                object scalarValue = mySqlCommand.ExecuteScalar();
                conn.Close();
                return Convert.ToInt32(scalarValue.ToString()) + 1;

            }
            catch
            {
                conn.Close();
                return 0;
            }

        }
        #endregion

       

        #region 부품 관련 함수

        public ObservableCollection<PartList> SelectAllDataPart()
        {

            ObservableCollection<PartList> list = new ObservableCollection<PartList>();

            try
            {
                conn.Open();

                var sql = "SELECT * FROM part_list ORDER BY indexNum ASC ";
                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        int indexNum = Convert.ToInt32(cursor["indexNum"]);
                        String Name = Convert.ToString(cursor["Name"]);
                        String Category = Convert.ToString(cursor["Category"]);            
                        int Size = Convert.ToInt32(cursor["Size"]);
                        Double Volume = Convert.ToDouble(cursor["Volume"]);
                        int Stock = Convert.ToInt32(cursor["Stock"]);
                        String Description = Convert.ToString(cursor["Description"]);
                        String Manufacturer = Convert.ToString(cursor["Manufacturer"]);
                        String Purchase = Convert.ToString(cursor["Purchase"]);
                        String Room = Convert.ToString(cursor["Room"]);
                        int Position = Convert.ToInt32(cursor["Position"]);
                        String Comment = Convert.ToString(cursor["Comment"]);

                        PartList Part = new PartList(indexNum, Name, Category, Size, Volume, Stock, Description, Manufacturer, Purchase, Room, Position, Comment);
                        list.Add(Part);
                    }
                    cursor.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            conn.Close();
            return list;
        }

        public List<String> SelectNameDataPart()
        {

            List<String> list = new List<String>();

            try
            {
                conn.Open();

                var sql = "SELECT Name FROM part_list ORDER BY indexNum ASC ";
                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        int indexNum = Convert.ToInt32(cursor["indexNum"]);
                        String Name = Convert.ToString(cursor["Name"]);
                        String Category = Convert.ToString(cursor["Category"]);
                        int Stock = Convert.ToInt32(cursor["Stock"]);
                        int Size = Convert.ToInt32(cursor["Size"]);
                        int Volume = Convert.ToInt32(cursor["Volume"]);
                        String Description = Convert.ToString(cursor["Description"]);
                        String Manufacturer = Convert.ToString(cursor["Manufacturer"]);
                        String Purchase = Convert.ToString(cursor["Purchase"]);
                        String Room = Convert.ToString(cursor["Room"]);
                        int Position = Convert.ToInt32(cursor["Position"]);
                        String Comment = Convert.ToString(cursor["Comment"]);


                        PartList Part = new PartList(indexNum, Name, Category, Size, Volume, Stock, Description, Manufacturer, Purchase, Room, Position, Comment);
                        list.Add(Name);
                    }
                    cursor.Close();
                }
            }
            catch
            {

            }

            conn.Close();
            return list;
        }

        public String InsertDataPart(PartList Part)
        {
            try
            {
                conn.Open();

                String sql = "INSERT INTO part_list VALUES (" + Part.indexNum + ",\"" + Part.Name + "\" , \"" + Part.Category + "\" , " + Part.Size + ", " + Part.Volume + ", " + Part.Stock + ", \"" + Part.Description + "\", \"" + Part.Manufacturer + "\", \"" + Part.Purchase + "\", \"" + Part.Room + "\", " + Part.Position + " , \"" + Part.Comment + "\" );";
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                conn.Close();
                return e.Message;
            }
            conn.Close();
            return "Part: " + Part.indexNum + "/" + Part.Name + " Data Insert Success!";

        }

        public String UpdateDataPart(PartList Part)
        {
            String sql = "UPDATE part_list SET Name=\"" + Part.Name + "\", Category=\"" + Part.Category + "\", Size= " + Part.Size +", Volume= " + Part.Volume + ", Stock= " + Part.Stock + ", Description=\"" + Part.Description + "\", Manufacturer=\"" + Part.Manufacturer + "\", Purchase=\"" + Part.Purchase + "\", Room=\"" + Part.Room + "\", Position=" + Part.Position + ", Comment=\"" + Part.Comment + "\" WHERE indexNum = " + Part.indexNum + " LIMIT 1; ";
            Debug.WriteLine(sql);
            try
            {
                conn.Open();
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
            }
            conn.Close();
            return "Part Name: " + Part.Name + " Update Success!";

        }

        public Boolean DeleteDataPart(PartList Part)
        {
            String sql = "DELETE FROM part_list WHERE indexNum = " + Part.indexNum + " LIMIT 1; ";
            Debug.WriteLine(sql);
            try
            {
                conn.Open();
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                conn.Close();
                return false;
            }
            conn.Close();
            return true;

        }

        public ObservableCollection<PartList> SearchFilterDataPart(String Category_S, String Serach)
        {

            ObservableCollection<PartList> list = new ObservableCollection<PartList>();

            string sql = "SELECT * From part_list WHERE ";

            sql += "Category LIKE \"%" + Category_S + "%\" AND (";
            sql += "Name LIKE \"%" + Serach + "%\" OR ";
            sql += "Description LIKE \"%" + Serach + "%\") ORDER BY indexNum ASC";

            try
            {
                conn.Open();



                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        int indexNum = Convert.ToInt32(cursor["indexNum"]);
                        String Name = Convert.ToString(cursor["Name"]);
                        String Category = Convert.ToString(cursor["Category"]);
                        int Stock = Convert.ToInt32(cursor["Stock"]);
                        int Size = Convert.ToInt32(cursor["Size"]);
                        Double Volume = Convert.ToDouble(cursor["Volume"]);
                        String Description = Convert.ToString(cursor["Description"]);
                        String Manufacturer = Convert.ToString(cursor["Manufacturer"]);
                        String Purchase = Convert.ToString(cursor["Purchase"]);
                        String Room = Convert.ToString(cursor["Room"]);
                        int Position = Convert.ToInt32(cursor["Position"]);
                        String Comment = Convert.ToString(cursor["Comment"]);


                        PartList Part = new PartList(indexNum, Name, Category, Size, Volume, Stock, Description, Manufacturer, Purchase, Room, Position, Comment);
                        list.Add(Part);
                    }
                    cursor.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + sql);
            }

            conn.Close();
            return list;
        }

        public int NewRoomNum(String Room)
        {
            try
            {
                conn.Open();
                var sql = "SELECT Position FROM part_list where Room = \""+ Room +"\" ORDER BY Position DESC LIMIT 1;";

                var mySqlCommand = new MySqlCommand(sql, conn);

                object scalarValue = mySqlCommand.ExecuteScalar();
                conn.Close();
                return Convert.ToInt32(scalarValue.ToString()) + 1;

            }
            catch
            {
                conn.Close();
                return 0;
            }

        }

        #endregion

        #region 구입처 관련 함수

        public ObservableCollection<PurchaseList> SelectAllDataPurchase()
        {

            ObservableCollection<PurchaseList> list = new ObservableCollection<PurchaseList>();

            try
            {
                conn.Open();

                var sql = "SELECT * FROM purchase_list ORDER BY indexNum ASC ";
                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        int indexNum = Convert.ToInt32(cursor["indexNum"]);
                        String Name = Convert.ToString(cursor["Name"]);
                        String Human = Convert.ToString(cursor["Human"]);
                        String Mail = Convert.ToString(cursor["Mail"]);
                        String Phone = Convert.ToString(cursor["Phone"]);
                        String Payment = Convert.ToString(cursor["Payment"]);
                        
                        String Comment = Convert.ToString(cursor["Comment"]);

                        PurchaseList purchase = new PurchaseList(indexNum, Name, Human, Mail, Phone, Payment, Comment);
                        list.Add(purchase);
                    }
                    cursor.Close();
                }
            }
            catch
            {

            }

            conn.Close();
            return list;
        }

        public ObservableCollection<String> SelectPurchaseDataName()
        {

            ObservableCollection<String> list = new ObservableCollection<String>();

            try
            {
                conn.Open();

                var sql = "SELECT Name FROM purchase_list ORDER BY indexNum ASC ";
                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        String Name = Convert.ToString(cursor["Name"]);
                        list.Add(Name);
                    }
                    cursor.Close();
                }
            }
            catch
            {

            }

            conn.Close();
            return list;
        }

        public String InsertDataPurchase(PurchaseList purchase)
        {
            try
            {
                conn.Open();

                String sql = "INSERT INTO purchase_list VALUES (" + purchase.indexNum + ",\"" + purchase.Name + "\" , \"" + purchase.Human + "\" , \"" + purchase.Mail + "\", \"" + purchase.Phone + "\",\"" + purchase.Payment + "\",\"" + purchase.Comment + "\" );";
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                conn.Close();
                return e.Message;
            }
            conn.Close();
            return "Purchase: " + purchase.indexNum + "/" + purchase.Name + " Data Insert Success!";

        }

        public String UpdateDataPurchase(PurchaseList purchase)
        {
            String sql = "UPDATE purchase_list SET Name=\"" + purchase.Name + "\", Human=\"" + purchase.Human + "\", Phone=\"" + purchase.Phone + "\", Mail=\"" + purchase.Mail + "\", Payment=\"" + purchase.Payment + "\", Comment=\"" + purchase.Comment +"\" WHERE indexNum = " + purchase.indexNum + " LIMIT 1; ";
            Debug.WriteLine(sql);
            try
            {
                conn.Open();
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
            }
            conn.Close();
            return "Purchase: " + purchase.Name + " Update Succees!";

        }

        public String UpdateDataPurchaseSort(PurchaseList purchase)
        {
            String sql = "UPDATE purchase_list SET IndexNum=" + purchase.indexNum + " WHERE Name = \"" + purchase.Name + "\" LIMIT 1; ";
            Debug.WriteLine(sql);
            try
            {
                conn.Open();
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
            }
            conn.Close();
            return "Purchase: " + purchase.Name+" Update Success!";

        }

        public Boolean DeleteDataPurchase(PurchaseList purchase)
        {
            String sql = "DELETE FROM purchase_list WHERE indexNum = " + purchase.indexNum + " LIMIT 1; ";
            Debug.WriteLine(sql);
            try
            {
                conn.Open();
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                conn.Close();
                return false;
            }
            conn.Close();
            return true;

        }

        public ObservableCollection<PurchaseList> SearchFilterDataPurchase(String Serach)
        {

            ObservableCollection<PurchaseList> list = new ObservableCollection<PurchaseList>();

            string sql = "SELECT * From purchase_list WHERE ";

            sql += "Name LIKE \"%" + Serach + "%\" OR ";
            sql += "Human LIKE \"%" + Serach + "%\" OR ";
            sql += "Mail LIKE \"%" + Serach + "%\" OR ";
            sql += "Phone LIKE \"%" + Serach + "%\" ORDER BY indexNum ASC";

            try
            {
                conn.Open();



                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        int indexNum = Convert.ToInt32(cursor["indexNum"]);
                        String Name = Convert.ToString(cursor["Name"]);
                        String Human = Convert.ToString(cursor["Human"]);
                        String Mail = Convert.ToString(cursor["Mail"]);
                        String Phone = Convert.ToString(cursor["Phone"]);
                        String Condition = Convert.ToString(cursor["Condition"]);
                        
                        String Comment = Convert.ToString(cursor["Comment"]);

                        PurchaseList purchase = new PurchaseList(indexNum, Name, Human, Mail, Phone, Condition, Comment);
                        list.Add(purchase);
                    }
                    cursor.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + sql);
            }

            conn.Close();
            return list;
        }

        #endregion

        #region 제조사 관련 함수

        public ObservableCollection<ManufacturerList> SelectAllDataManufacturer()
        {

            ObservableCollection<ManufacturerList> list = new ObservableCollection<ManufacturerList>();

            try
            {
                conn.Open();

                var sql = "SELECT * FROM manufacturer_list ORDER BY indexNum ASC ";
                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        int indexNum = Convert.ToInt32(cursor["indexNum"]);
                        String Name = Convert.ToString(cursor["Name"]);
                        String Purchase = Convert.ToString(cursor["Purchase"]);
                        String Comment = Convert.ToString(cursor["Comment"]);

                        ManufacturerList manufacturer = new ManufacturerList(indexNum, Name, Purchase, Comment);
                        list.Add(manufacturer);
                    }
                    cursor.Close();
                }
            }
            catch
            {

            }

            conn.Close();
            return list;
        }

        public String InsertDataManufacturer(ManufacturerList manufacturer)
        {
            try
            {
                conn.Open();

                String sql = "INSERT INTO manufacturer_list VALUES (" + manufacturer.indexNum + ",\"" + manufacturer.Name + "\" , \"" + manufacturer.Purchase + "\", \""+manufacturer.Comment + "\" );";
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                conn.Close();
                return e.Message;
            }
            conn.Close();
            return "Manufacturer: " + manufacturer.indexNum +"/" + manufacturer.Name + " Data Insert Success!";

        }

        public String UpdateDataManufacturer(ManufacturerList manufacturer)
        {
            String sql = "UPDATE manufacturer_list SET Name=\"" + manufacturer.Name + "\", Purchase=\"" + manufacturer.Purchase + "\", Comment= \"" + manufacturer.Comment + "\" WHERE indexNum = " + manufacturer.indexNum + " LIMIT 1; ";
            Debug.WriteLine(sql);
            try
            {
                conn.Open();
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
            }
            conn.Close();
            return "Manufacturer: " + manufacturer.Name + " Update Success!";

        }

        public String UpdateDataManufacturerSort(ManufacturerList manufacturer)
        {
            String sql = "UPDATE manufacturer_list SET IndexNum=" + manufacturer.indexNum + " WHERE Name = \"" + manufacturer.Name + "\" LIMIT 1; ";
            Debug.WriteLine(sql);
            try
            {
                conn.Open();
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
            }
            conn.Close();
            return "Manufacturer: " + manufacturer.Name + " Update Success!";

        }

        public Boolean DeleteDataManufacturer(ManufacturerList manufacturer)
        {
            String sql = "DELETE FROM manufacturer_list WHERE indexNum = " + manufacturer.indexNum + " LIMIT 1; ";
            Debug.WriteLine(sql);
            try
            {
                conn.Open();
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                conn.Close();
                return false;
            }
            conn.Close();
            return true;

        }

        public ObservableCollection<ManufacturerList> SearchFilterDataManufacturer(String Serach)
        {

            ObservableCollection<ManufacturerList > list = new ObservableCollection<ManufacturerList> ();

            string sql = "SELECT * From manufacturer_list WHERE ";
            sql += "Name LIKE \"%" + Serach + "%\" OR ";
            sql += "Purchase LIKE \"%" + Serach + "%\" ORDER BY indexNum ASC";

            try
            {
                conn.Open();

                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        int indexNum = Convert.ToInt32(cursor["indexNum"]);
                        String Name = Convert.ToString(cursor["Name"]);
                        String Purchase = Convert.ToString(cursor["Purchase"]);
                        String Comment = Convert.ToString(cursor["Comment"]);

                        ManufacturerList manufacturer = new ManufacturerList(indexNum, Name, Purchase, Comment);
                        list.Add(manufacturer);
                    }
                    cursor.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + sql);
            }

            conn.Close();
            return list;
        }

        public List<String> SelectManufacturerDataName()
        {

            List<String> list = new List<String>();

            try
            {
                conn.Open();

                var sql = "SELECT Name FROM manufacturer_list ORDER BY indexNum ASC ";
                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        String Name = Convert.ToString(cursor["Name"]);
                        list.Add(Name);
                    }
                    cursor.Close();
                }
            }
            catch
            {

            }

            conn.Close();
            return list;
        }

        #endregion



        #region 카테고리 관련 함수

        public List<CategoryList> SelectAllDataCategory()
        {

            List<CategoryList> list = new List<CategoryList>();

            try
            {
                conn.Open();

                var sql = "SELECT * FROM category_list ORDER BY indexNum ASC ";
                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        int indexNum = Convert.ToInt32(cursor["indexNum"]);
                        String Name = Convert.ToString(cursor["Name"]);

                        CategoryList Category = new CategoryList(indexNum, Name);
                        list.Add(Category);
                    }
                    cursor.Close();
                }
            }
            catch
            {

            }

            conn.Close();
            return list;
        }

        public String InsertDataCategory(CategoryList Category)
        {
            try
            {
                conn.Open();

                String sql = "INSERT INTO category_list VALUES (" + Category.indexNum + ",\"" + Category.Name + "\" );";
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                conn.Close();
                return e.Message;
            }
            conn.Close();
            return "Category: " + Category.indexNum + "/" + Category.Name + " Data Insert Success!";

        }

        public String UpdateDataCategory(CategoryList Category)
        {
            String sql = "UPDATE category_list SET Name=\"" + Category.Name + "\" WHERE indexNum = " + Category.indexNum + " LIMIT 1; ";
            Debug.WriteLine(sql);
            try
            {
                conn.Open();
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
            }
            conn.Close();
            return "Category: " + Category.Name + " Update Success!";

        }

        public Boolean DeleteDataCategory(CategoryList Category)
        {
            String sql = "DELETE FROM category_list WHERE indexNum = " + Category.indexNum + " LIMIT 1; ";
            Debug.WriteLine(sql);
            try
            {
                conn.Open();
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                conn.Close();
                return false;
            }
            conn.Close();
            return true;

        }

        public List<CategoryList> SearchFilterDataCategory(String Serach)
        {

            List<CategoryList> list = new List<CategoryList>();

            string sql = "SELECT * From category_list WHERE ";
            sql += "Name LIKE \"%" + Serach + "%\" ORDER BY indexNum ASC";

            try
            {
                conn.Open();

                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        int indexNum = Convert.ToInt32(cursor["indexNum"]);
                        String Name = Convert.ToString(cursor["Name"]);

                        CategoryList Category = new CategoryList(indexNum, Name);
                        list.Add(Category);
                    }
                    cursor.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + sql);
            }

            conn.Close();
            return list;
        }

        public List<String> SelectCategoryDataName()
        {

            List<String> list = new List<String>();

            try
            {
                conn.Open();

                var sql = "SELECT Name FROM category_list ORDER BY indexNum ASC ";
                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        String Name = Convert.ToString(cursor["Name"]);
                        list.Add(Name);
                    }
                    cursor.Close();
                }
            }
            catch
            {

            }

            conn.Close();
            return list;
        }

        #endregion

        public List<LoggerList> SelectAllDataLogger()
        {

            List<LoggerList> list = new List<LoggerList>();

            try
            {
                conn.Open();

                var sql = "SELECT * FROM logger_list ORDER BY indexNum ASC ";
                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        int indexNum = Convert.ToInt32(cursor["indexNum"]);
                        DateTime inputTime = Convert.ToDateTime(cursor["InputTime"]);
                        String Comment = Convert.ToString(cursor["Comment"]);

                        LoggerList Logger = new LoggerList(indexNum, inputTime, Comment);
                        list.Add(Logger);
                    }
                    cursor.Close();
                }
            }
            catch
            {

            }

            conn.Close();
            return list;
        }

        public String InsertDataLogger(LoggerList Logger)
        {
            try
            {
                conn.Open();

                String sql = "INSERT INTO logger_list VALUES (" + Logger.indexNum + ",now(), \"" + Logger.Comment + "\" );";
                var mySqlCommand = new MySqlCommand(sql, conn);
                mySqlCommand.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                conn.Close();
                return e.Message;
            }
            conn.Close();
            return Logger.indexNum + " Data Insert Success!";

        }

        public List<LoggerList> SearchFilterDataLogger(DateTime date1, DateTime date2, String Serach)
        {

            List<LoggerList> list = new List<LoggerList>();

            string sql = "SELECT * From logger_list WHERE ";
            sql += "DATE(InputTime) BETWEEN \"" + date1 + "\" AND \"" + date2 + "\" AND ";
            sql += "Comment LIKE \"%" + Serach + "%\" ORDER BY indexNum ASC";

            try
            {
                conn.Open();

                var mySqlCommand = new MySqlCommand(sql, conn);

                using (var cursor = mySqlCommand.ExecuteReader())
                {
                    while (cursor.Read())
                    {
                        int indexNum = Convert.ToInt32(cursor["indexNum"]);
                        DateTime InputTime = Convert.ToDateTime(cursor["InputTime"]);
                        String Comment = Convert.ToString(cursor["Comment"]);

                        LoggerList Logger = new LoggerList(indexNum, InputTime, Comment);
                        list.Add(Logger);
                    }
                    cursor.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + sql);
            }

            conn.Close();
            return list;
        }



    }
}