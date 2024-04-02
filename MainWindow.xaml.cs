using System.Windows;
using MahApps.Metro.Controls;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        DataManagement dataManagement = new DataManagement(new MySqlConnection("server=127.0.0.1;user=root;database=projectdb;password=1234;CharSet=utf8;"));

        public MainWindow()
        {
            InitializeComponent();
        
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tap_Main_Init();
            DataGrid_Part_Init();
            DataGrid_Category_Init();
            DataGrid_Manufacturer_Init();
            DataGrid_Purchase_Init();
            DataGrid_Logger_Init();
        }

        #region window 창 관련 함수

        private void DeployCupCakes(object sender, RoutedEventArgs e)
        {
            Setting.IsEnabled = true;
        }

        private void LaunchGitHubSite(object sender, RoutedEventArgs e)
        {

        }



        #endregion

        #region 메인 탭 관련 함수

        public void Tap_Main_Init()
        {
        }

        #endregion

        #region 부품 관련 함수

        ObservableCollection<PartList> PartList = null;
        List<String> Part_CategoryDataNameList, Part_ManufacturerDataNameList;
        ObservableCollection<String> Part_PurchaseDataNameList = null;
        private Boolean isManualEditCommit_Part;

        private void DataGrid_Part_NewPosition_btn_Click(object sender, RoutedEventArgs e)
        {
            PartList list = DataGrid_Part.SelectedItem as PartList;

            if(list.Position == 0 && list != null)
            {
                list.Position = dataManagement.NewRoomNum(list.Room);
                Logger_WriteLine(dataManagement.UpdateDataPart(list));
                PartList[DataGrid_Part.SelectedIndex] = list;
                DataGrid_Part.ItemsSource = null;
                DataGrid_Part.ItemsSource = PartList;
            }

        }

        public void DataGrid_Part_Init()
        {
            DataGrid_Part_SubItem_Add();
            DataGrid_Part_DataSheet_init();
        }

        public void DataGrid_Part_DataSheet_init()
        {
            //List<Boolean> Part_DataSheet_Exists_Check = new List<Boolean>();

            for (int i=0;i< PartList.Count;i++)
            {
                FileInfo di = new FileInfo("datasheet\\" + PartList[i].Name + "\\"+ PartList[i].Name+".pdf");
                PartList[i].Part_DataSheet_Exists_Check = di.Exists;
            }

            DataGrid_Part.ItemsSource = PartList;

        }

        public void DataGrid_Part_SubItem_Add()
        {
            //Logger_WriteLine("[DataGrid_Part_SubItem_Add() 시작]");

            try
            {
                PartList = dataManagement.SelectAllDataPart();
                DataGrid_Part.ItemsSource = PartList;

                Part_CategoryDataNameList = dataManagement.SelectCategoryDataName();
                Part_Category_ComboBox.ItemsSource = Part_CategoryDataNameList;

                DataGrid_Part.SelectedIndex = DataGrid_Part.Items.Count - 1;
                DataGrid_Part_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
            //Logger_WriteLine("[DataGrid_Part_SubItem_Add() 종료]");
        }

        public void DataGrid_Part_SubItem_Add(int indexNum)
        {
            //Logger_WriteLine("[DataGrid_Part_SubItem_Add() 시작]");

            try
            {
                PartList.Add(new PartList(indexNum, "", Part_Category_comboBox.Text, 0,0,1,"","","","",0,""));
                Logger_WriteLine(dataManagement.InsertDataPart(PartList[PartList.Count - 1]));

                DataGrid_Part.SelectedIndex = DataGrid_Part.Items.Count - 1;
                DataGrid_Part.ScrollIntoView(PartList[PartList.Count - 1]);
                DataGrid_Part_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
            //Logger_WriteLine("[DataGrid_Part_SubItem_Add() 종료]");
        }

        private void DataGrid_Part_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            int FocusNum = dataManagement.NewIndexNum("Part_list");
            DataGrid_Part_SubItem_Add(FocusNum);

        }

        private void DataGrid_Part_Sub_Button_Click(object sender, RoutedEventArgs e)
        {
            int SelectedIndex = DataGrid_Part.SelectedIndex;
            try
            {
                if (MessageBox.Show(PartList[SelectedIndex].indexNum + "번 " + PartList[SelectedIndex].Name + "를 삭제하시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DataGrid_Part_SubItem_Sub(SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
        }

        private void DataGrid_Part_Data_Changed()
        {
            DataGrid_PartList_ItemCount_txtb.Text = PartList.Count + " Data(s) find";
            DataGrid_Part_DataSheet_init();
        }

        public void DataGrid_Part_SubItem_Sub(int SelectedIndex)
        {
            //Logger_WriteLine("[DataGrid_Part_SubItem_Sub() 시작]");

            try
            {
                PartList Part = DataGrid_Part.SelectedItem as PartList;

                if (dataManagement.DeleteDataPart(Part))
                {
                    PartList.RemoveAt(SelectedIndex);

                    if (SelectedIndex >= PartList.Count) SelectedIndex = PartList.Count - 1;
                    DataGrid_Part.SelectedIndex = SelectedIndex;
                    DataGrid_Part.ScrollIntoView(PartList[SelectedIndex]);
                    DataGrid_Part_Data_Changed();
                    Logger_WriteLine(Part.indexNum + "번 " + Part.Name + " 삭제완료!/" + SelectedIndex);
                }
                else
                {
                    Logger_WriteLine("삭제실패!");
                }

            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }

            //Logger_WriteLine("[DataGrid_Part_SubItem_Sub() 종료]");
        }

        private void DataGrid_Part_BeginningEdit(object sender, System.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            if (DataGrid_Part.CurrentColumn.DisplayIndex == 0)
            {
                e.Cancel = true;

                DirectoryInfo di = new DirectoryInfo("datasheet\\" + PartList[DataGrid_Part.SelectedIndex].Name);
                if (di.Exists == false)
                {
                    di.Create();
                    Process.Start("datasheet\\" + PartList[DataGrid_Part.SelectedIndex].Name);
                }
                else
                {
                    FileInfo di2 = new FileInfo("datasheet\\" + PartList[DataGrid_Part.SelectedIndex].Name + "\\" + PartList[DataGrid_Part.SelectedIndex].Name + ".pdf");
                    if(di2.Exists == true)
                    {
                        Process.Start("datasheet\\" + PartList[DataGrid_Part.SelectedIndex].Name + "\\" + PartList[DataGrid_Part.SelectedIndex].Name + ".pdf");
                    }
                    else
                    {
                        Process.Start("datasheet\\" + PartList[DataGrid_Part.SelectedIndex].Name);
                    }
                }

            }
            
        }

        private void DataGrid_Part_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            if (!isManualEditCommit_Part)
            {
                isManualEditCommit_Part = true;
                DataGrid grid = (DataGrid)sender;
                grid.CommitEdit(DataGridEditingUnit.Row, true);
                isManualEditCommit_Part = false;
                Logger_WriteLine(dataManagement.UpdateDataPart(DataGrid_Part.SelectedItem as PartList));
            }

        }

        private void DataGrid_Part_Filter_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                PartList = dataManagement.SearchFilterDataPart(Part_Category_comboBox.Text,DataGrid_Part_Filter_TextBox.Text);
                DataGrid_Part.ItemsSource = PartList;
                DataGrid_Part_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }

        }

        private void DataGrid_Part_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Part_CategoryDataNameList = dataManagement.SelectCategoryDataName();
            Part_ManufacturerDataNameList = dataManagement.SelectManufacturerDataName();
            Part_PurchaseDataNameList = dataManagement.SelectPurchaseDataName();

            Part_Category_ComboBox.ItemsSource = Part_CategoryDataNameList;
            Part_Manufacturer_ComboBox.ItemsSource = Part_ManufacturerDataNameList;
            Part_Purchase_ComboBox.ItemsSource = Part_PurchaseDataNameList;

        }

        private void Part_Category_comboBox_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Part_Category_comboBox.ItemsSource = Part_CategoryDataNameList;
        }

        private void Part_Category_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string text = (sender as ComboBox).SelectedItem as string;
                PartList = dataManagement.SearchFilterDataPart(text, DataGrid_Part_Filter_TextBox.Text);
                DataGrid_Part.ItemsSource = PartList;
                DataGrid_Part_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
        }

        private void DataGrid_Part_columnHeader_Click(object sender, RoutedEventArgs e)
        {
            //var ColunmHeader = sender as System.Windows.Controls.Primitives.DataGridColumnHeader;
            
            //Logger_WriteLine(ColunmHeader.Name.ToString());
 
        }

        #endregion

        #region TapMenu 카테고리

        List<CategoryList> CategoryList = null;
        String CategoryEditTemp;
        int CategorySelectedRow, CategorySelectedColumn = 0;

        public void DataGrid_Category_Init()
        {
            DataGrid_Category_SubItem_Add();
        }

        public void DataGrid_Category_SubItem_Add()
        {
            //Logger_WriteLine("[DataGrid_Category_SubItem_Add() 시작]");

            try
            {
                CategoryList = dataManagement.SelectAllDataCategory();
                DataGrid_Category.ItemsSource = CategoryList;

                DataGrid_Category.SelectedIndex = DataGrid_Category.Items.Count - 1;
                DataGrid_Category_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
            //Logger_WriteLine("[DataGrid_Category_SubItem_Add() 종료]");
        }

        public void DataGrid_Category_SubItem_Add(int indexNum)
        {
            //Logger_WriteLine("[DataGrid_Category_SubItem_Add() 시작]");

            try
            {
                CategoryList.Add(new CategoryList(indexNum, ""));
                Logger_WriteLine(dataManagement.InsertDataCategory(CategoryList[CategoryList.Count - 1]));
                DataGrid_Category.ItemsSource = null;
                DataGrid_Category.ItemsSource = CategoryList;

                //Category_Purchase_ComboBox.ItemsSource = PurchaseDataNameList;

                DataGrid_Category.SelectedIndex = DataGrid_Category.Items.Count - 1;
                DataGrid_Category.ScrollIntoView(CategoryList[CategoryList.Count - 1]);
                DataGrid_Category_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
            //Logger_WriteLine("[DataGrid_Category_SubItem_Add() 종료]");
        }

        private void DataGrid_Category_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            int FocusNum = dataManagement.NewIndexNum("Category_list");
            DataGrid_Category_SubItem_Add(FocusNum);

        }

        private void DataGrid_Category_Sub_Button_Click(object sender, RoutedEventArgs e)
        {
            int SelectedIndex = DataGrid_Category.SelectedIndex;
            try
            {
                if (MessageBox.Show(CategoryList[SelectedIndex].indexNum + "번 " + CategoryList[SelectedIndex].Name + "를 삭제하시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DataGrid_Category_SubItem_Sub(SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
        }

        private void DataGrid_Category_Data_Changed()
        {
            DataGrid_CategoryList_ItemCount_txtb.Text = CategoryList.Count + " Data(s) find";
        }

        public void DataGrid_Category_SubItem_Sub(int SelectedIndex)
        {
            //Logger_WriteLine("[DataGrid_Category_SubItem_Sub() 시작]");

            try
            {
                CategoryList Category = DataGrid_Category.SelectedItem as CategoryList;

                if (dataManagement.DeleteDataCategory(Category))
                {
                    CategoryList.RemoveAt(SelectedIndex);
                    DataGrid_Category.ItemsSource = null;
                    DataGrid_Category.ItemsSource = CategoryList;
                    if (SelectedIndex >= CategoryList.Count) SelectedIndex = CategoryList.Count - 1;
                    DataGrid_Category.SelectedIndex = SelectedIndex;
                    DataGrid_Category.ScrollIntoView(CategoryList[SelectedIndex]);
                    DataGrid_Category_Data_Changed();
                    Logger_WriteLine(Category.indexNum + "번 " + Category.Name + " 삭제완료!/" + SelectedIndex);
                }
                else
                {
                    Logger_WriteLine("삭제실패!");
                }

            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }

            //Logger_WriteLine("[DataGrid_Category_SubItem_Sub() 종료]");
        }

        private void DataGrid_Category_BeginningEdit(object sender, System.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            CategorySelectedRow = DataGrid_Category.SelectedIndex;
            CategorySelectedColumn = DataGrid_Category.CurrentColumn.DisplayIndex;

            switch (CategorySelectedColumn)
            {
                case 0:
                    CategoryEditTemp = CategoryList[CategorySelectedRow].indexNum + "";
                    break;
                case 1:
                    CategoryEditTemp = CategoryList[CategorySelectedRow].Name + "";
                    break;
                default:
                    CategoryEditTemp = "";
                    break;
            }

            Logger_WriteLine(CategoryEditTemp);

            if (((DataGrid)sender).CurrentCell.Column is System.Windows.Controls.DataGridTextColumn)
            {

            }
            else if (((DataGrid)sender).CurrentCell.Column is System.Windows.Controls.DataGridComboBoxColumn)
            {

            }
            else
            {
                e.Cancel = true;
                Logger_WriteLine("BeginningEdit 아님/" + ((DataGrid)sender).CurrentCell.Column);
            }
        }

        private void DataGrid_Category_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            try
            {
                String editedCellValue = "";
                //Logger_WriteLine(e.EditingElement.ToString());
                if (e.EditingElement is System.Windows.Controls.TextBox)
                {
                    System.Windows.Controls.TextBox t = e.EditingElement as System.Windows.Controls.TextBox;
                    editedCellValue = t.Text.ToString();
                }
                else if ((e.EditingElement is System.Windows.Controls.ComboBox))
                {
                    System.Windows.Controls.ComboBox t = e.EditingElement as System.Windows.Controls.ComboBox;
                    editedCellValue = t.Text.ToString();
                }
                else
                {

                }

                CategoryList Category = DataGrid_Category.SelectedItem as CategoryList;
                switch (CategorySelectedColumn)
                {
                    case 0:
                        Category.indexNum = Convert.ToInt32(editedCellValue);
                        break;
                    case 1:
                        Category.Name = editedCellValue;
                        break;
                    default:
                        break;
                }

                Logger_WriteLine(dataManagement.UpdateDataCategory(Category));

                switch (CategorySelectedColumn)
                {
                    case 0:
                        CategoryList[CategorySelectedRow].indexNum = Convert.ToInt32(editedCellValue);
                        break;
                    case 1:
                        CategoryList[CategorySelectedRow].Name = editedCellValue;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }


        }

        private void DataGrid_Category_Filter_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                CategoryList = dataManagement.SearchFilterDataCategory(DataGrid_Category_Filter_TextBox.Text);
                DataGrid_Category.ItemsSource = CategoryList;
                DataGrid_Category_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }

        }

        private void DataGrid_Category_Up_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int SelectedIndex = DataGrid_Category.SelectedIndex;

                CategoryList temp1 = DataGrid_Category.SelectedItem as CategoryList;
                CategoryList temp2 = DataGrid_Category.Items[SelectedIndex - 1] as CategoryList;

                int tempIndexNum = temp1.indexNum;
                temp1.indexNum = temp2.indexNum;
                temp2.indexNum = tempIndexNum;

                dataManagement.UpdateDataCategory(temp1);
                dataManagement.UpdateDataCategory(temp2);

                CategoryList.RemoveAt(SelectedIndex);
                CategoryList.Insert(SelectedIndex - 1, temp1);

                DataGrid_Category.ItemsSource = null;
                DataGrid_Category.ItemsSource = CategoryList;

                DataGrid_Category.SelectedIndex = SelectedIndex - 1;
                DataGrid_Category.ScrollIntoView(CategoryList[SelectedIndex - 1]);

            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
        }

        private void DataGrid_Category_Down_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int SelectedIndex = DataGrid_Category.SelectedIndex;

                CategoryList temp1 = DataGrid_Category.SelectedItem as CategoryList;
                CategoryList temp2 = DataGrid_Category.Items[SelectedIndex + 1] as CategoryList;

                int tempIndexNum = temp1.indexNum;
                temp1.indexNum = temp2.indexNum;
                temp2.indexNum = tempIndexNum;

                dataManagement.UpdateDataCategory(temp1);
                dataManagement.UpdateDataCategory(temp2);

                CategoryList.RemoveAt(SelectedIndex);
                CategoryList.Insert(SelectedIndex + 1, temp1);

                DataGrid_Category.ItemsSource = null;
                DataGrid_Category.ItemsSource = CategoryList;

                DataGrid_Category.SelectedIndex = SelectedIndex + 1;
                DataGrid_Category.ScrollIntoView(CategoryList[SelectedIndex + 1]);

            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
        }

        #endregion

        #region TapMenu 제조사

        ObservableCollection<ManufacturerList> ManufacturerList = null;
        ObservableCollection<String> PurchaseDataNameList = null;
        private Boolean isManualEditCommit;

        public void DataGrid_Manufacturer_Init()
        {
            DataGrid_Manufacturer_SubItem_Add();
        }

        public void DataGrid_Manufacturer_SubItem_Add()
        {
            try
            {
                ManufacturerList = dataManagement.SelectAllDataManufacturer();
                DataGrid_Manufacturer.ItemsSource = ManufacturerList;

                Manufacturer_Purchase_ComboBox.ItemsSource = dataManagement.SelectPurchaseDataName();

                DataGrid_Manufacturer.SelectedIndex = DataGrid_Manufacturer.Items.Count - 1;
                DataGrid_Manufacturer_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
        }

        public void DataGrid_Manufacturer_SubItem_Add(int indexNum)
        {
            try
            {
                ManufacturerList.Add(new ManufacturerList(indexNum, "", "",""));
                Logger_WriteLine(dataManagement.InsertDataManufacturer(ManufacturerList[ManufacturerList.Count - 1]));

                PurchaseDataNameList = dataManagement.SelectPurchaseDataName();
                Manufacturer_Purchase_ComboBox.ItemsSource = PurchaseDataNameList;

                DataGrid_Manufacturer.SelectedIndex = DataGrid_Manufacturer.Items.Count - 1;
                DataGrid_Manufacturer.ScrollIntoView(ManufacturerList[ManufacturerList.Count - 1]);
                DataGrid_Manufacturer_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
        }

        private void DataGrid_Manufacturer_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            int FocusNum = dataManagement.NewIndexNum("Manufacturer_list");
            DataGrid_Manufacturer_SubItem_Add(FocusNum);

        }

        private void DataGrid_Manufacturer_Sub_Button_Click(object sender, RoutedEventArgs e)
        {
            int SelectedIndex = DataGrid_Manufacturer.SelectedIndex;
            try
            {
                if (MessageBox.Show(ManufacturerList[SelectedIndex].indexNum + "번 " + ManufacturerList[SelectedIndex].Name + "를 삭제하시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DataGrid_Manufacturer_SubItem_Sub(SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
        }

        private void DataGrid_Manufacturer_Data_Changed()
        {
            DataGrid_ManufacturerList_ItemCount_txtb.Text = ManufacturerList.Count + " Data(s) find";
        }

        public void DataGrid_Manufacturer_SubItem_Sub(int SelectedIndex)
        {
            //Logger_WriteLine("[DataGrid_Manufacturer_SubItem_Sub() 시작]");

            try
            {
                ManufacturerList Manufacturer = DataGrid_Manufacturer.SelectedItem as ManufacturerList;

                if (dataManagement.DeleteDataManufacturer(Manufacturer))
                {
                    ManufacturerList.RemoveAt(SelectedIndex);
                    if (SelectedIndex >= ManufacturerList.Count) SelectedIndex = ManufacturerList.Count - 1;
                    DataGrid_Manufacturer.SelectedIndex = SelectedIndex;
                    DataGrid_Manufacturer.ScrollIntoView(ManufacturerList[SelectedIndex]);
                    DataGrid_Manufacturer_Data_Changed();
                    Logger_WriteLine(Manufacturer.indexNum + "번 " + Manufacturer.Name + " 삭제완료!/" + SelectedIndex);
                }
                else
                {
                    Logger_WriteLine("삭제실패!");
                }

            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
        }        

        private void DataGrid_Manufacturer_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            if (!isManualEditCommit)
            {
                isManualEditCommit = true;
                DataGrid grid = (DataGrid)sender;
                grid.CommitEdit(DataGridEditingUnit.Row, true);
                isManualEditCommit = false;
                Logger_WriteLine(dataManagement.UpdateDataManufacturer(DataGrid_Manufacturer.SelectedItem as ManufacturerList));
            }
        }

        private void DataGrid_Manufacturer_Filter_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ManufacturerList = dataManagement.SearchFilterDataManufacturer(DataGrid_Manufacturer_Filter_TextBox.Text);
                DataGrid_Manufacturer.ItemsSource = ManufacturerList;
                DataGrid_Manufacturer_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }

        }

        private void DataGrid_Manufacturer_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PurchaseDataNameList = dataManagement.SelectPurchaseDataName();
            Manufacturer_Purchase_ComboBox.ItemsSource = PurchaseDataNameList;
        }

        private void DataGrid_Manufacturer_Sort_Button_Click(object sender, RoutedEventArgs e)
        {
            List<ManufacturerList> SortedList = new List<ManufacturerList>();

            for(int i = 0; i< ManufacturerList.Count; i++)
            {
                SortedList.Add(new ManufacturerList(ManufacturerList[i].indexNum, ManufacturerList[i].Name, ManufacturerList[i].Purchase, ManufacturerList[i].Comment));          
            }

            List<ManufacturerList> SortedList1 = SortedList.OrderBy(x => x.Name).ToList();

            for (int i = 0; i < SortedList1.Count; i++)
            {
                SortedList1[i].indexNum = i + 1;
                Logger_WriteLine(dataManagement.UpdateDataManufacturerSort(SortedList1[i]));
            }

            ManufacturerList = dataManagement.SelectAllDataManufacturer();
            DataGrid_Manufacturer.ItemsSource = ManufacturerList;
        }

        #endregion

        #region TapMenu 구입처

        ObservableCollection<PurchaseList> purchaseList = null;
        private Boolean isManualEditCommit_Purchase;

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            Random rnd = new Random();
            int num = rnd.Next(purchaseList.Count);

            ManufacturerList[DataGrid_Manufacturer.SelectedIndex].Purchase = purchaseList[num].Name;

            

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ManufacturerList list = DataGrid_Manufacturer.SelectedItem as ManufacturerList;
            Logger_WriteLine(list.Name + "/" + list.Purchase + "\n" + ManufacturerList[DataGrid_Manufacturer.SelectedIndex].Name + "/" + ManufacturerList[DataGrid_Manufacturer.SelectedIndex].Purchase);
        }


        public void DataGrid_Purchase_Init()
        {
            DataGrid_Purchase_SubItem_Add();
        }

        public void DataGrid_Purchase_SubItem_Add()
        {
            //Logger_WriteLine("[DataGrid_Purchase_SubItem_Add() 시작]");

            try
            {
                purchaseList = dataManagement.SelectAllDataPurchase();
                DataGrid_Purchase.ItemsSource = purchaseList;
                DataGrid_Purchase.SelectedIndex = DataGrid_Purchase.Items.Count - 1;
                DataGrid_Purchase_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
            //Logger_WriteLine("[DataGrid_Purchase_SubItem_Add() 종료]");
        }

        public void DataGrid_Purchase_SubItem_Add(int indexNum)
        {
            //Logger_WriteLine("[DataGrid_Purchase_SubItem_Add() 시작]");

            try
            {
                purchaseList.Add(new PurchaseList(indexNum, "", "", "", "","",""));
                Logger_WriteLine(dataManagement.InsertDataPurchase(purchaseList[purchaseList.Count - 1]));
                DataGrid_Purchase.SelectedIndex = DataGrid_Purchase.Items.Count - 1;
                DataGrid_Purchase.ScrollIntoView(purchaseList[purchaseList.Count - 1]);
                DataGrid_Purchase_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
            //Logger_WriteLine("[DataGrid_Purchase_SubItem_Add() 종료]");
        }

        private void DataGrid_Purchase_Add_Button_Click(object sender, EventArgs e)
        {
            int FocusNum = dataManagement.NewIndexNum("purchase_list");
            DataGrid_Purchase_SubItem_Add(FocusNum);
            
        }

        private void DataGrid_Purchase_Sub_Button_Click(object sender, EventArgs e)
        {
            int SelectedIndex = DataGrid_Purchase.SelectedIndex;
            try
            {
                if (MessageBox.Show(purchaseList[SelectedIndex].indexNum + "번 " + purchaseList[SelectedIndex].Name + "를 삭제하시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DataGrid_Purchase_SubItem_Sub(SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
        }

        private void DataGrid_Purchase_Sort_Button_Click(object sender, RoutedEventArgs e)
        {
            List<PurchaseList> SortedList = new List<PurchaseList>();

            for (int i = 0; i < purchaseList.Count; i++)
            {
                SortedList.Add(new PurchaseList(purchaseList[i].indexNum, purchaseList[i].Name, purchaseList[i].Human, purchaseList[i].Mail, purchaseList[i].Phone, purchaseList[i].Payment, purchaseList[i].Comment));
            }

            List<PurchaseList> SortedList1 = SortedList.OrderBy(x => x.Name).ToList();

            for (int i = 0; i < SortedList1.Count; i++)
            {
                SortedList1[i].indexNum = i + 1;
                Logger_WriteLine(dataManagement.UpdateDataPurchaseSort(SortedList1[i]));
            }

            purchaseList = dataManagement.SelectAllDataPurchase();
            DataGrid_Purchase.ItemsSource = purchaseList;

            DataGrid_Purchase_Data_Changed();
        }

        

        private void DataGrid_Purchase_Data_Changed()
        {
            dataGrid_PurchaseList_ItemCount_txtb.Text = purchaseList.Count + " Data(s) find";
        }

        public void DataGrid_Purchase_SubItem_Sub(int SelectedIndex)
        {
            //Logger_WriteLine("[DataGrid_Purchase_SubItem_Sub() 시작]");

            try
            {
                PurchaseList purchase = DataGrid_Purchase.SelectedItem as PurchaseList;

                if (dataManagement.DeleteDataPurchase(purchase))
                {
                    purchaseList.RemoveAt(SelectedIndex);

                    if (SelectedIndex >= purchaseList.Count) SelectedIndex = purchaseList.Count - 1;
                    DataGrid_Purchase.SelectedIndex = SelectedIndex;
                    DataGrid_Purchase.ScrollIntoView(purchaseList[SelectedIndex]);
                    DataGrid_Purchase_Data_Changed();
                    Logger_WriteLine(purchase.indexNum + "번 " + purchase.Name + " 삭제완료!/" + SelectedIndex);
                }
                else
                {
                    Logger_WriteLine("삭제실패!");
                }

            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }

            //Logger_WriteLine("[DataGrid_Purchase_SubItem_Sub() 종료]");
        }
        
        

        private void DataGrid_Purchase_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {

            if (!isManualEditCommit_Purchase)
            {
                isManualEditCommit_Purchase = true;
                DataGrid grid = (DataGrid)sender;
                grid.CommitEdit(DataGridEditingUnit.Row, true);
                isManualEditCommit_Purchase = false;
                Logger_WriteLine(dataManagement.UpdateDataPurchase(DataGrid_Purchase.SelectedItem as PurchaseList));
            }

        }      

        private void DataGrid_Purchase_Filter_TextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                purchaseList = dataManagement.SearchFilterDataPurchase(DataGrid_Purchase_Filter_TextBox.Text);

                DataGrid_Purchase.ItemsSource = purchaseList;
                DataGrid_Purchase_Data_Changed();
            }
            catch(Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
            
        }

        #endregion

        #region TapMenu 로그

        List<LoggerList> LoggerList = null;


        public void DataGrid_Logger_Init()
        {
            Logger_DatePicker_1.DisplayDateEnd = DateTime.Today;
            Logger_DatePicker_2.DisplayDateEnd = DateTime.Today;

            Logger_DatePicker_1.SelectedDate = DateTime.Today;
            Logger_DatePicker_2.SelectedDate = DateTime.Today;

            DataGrid_Logger_SubItem_Add();
        }

        private void Logger_DatePicker_2_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Logger_DatePicker_1.DisplayDateEnd = Logger_DatePicker_2.SelectedDate;
            try
            {

                LoggerList = dataManagement.SearchFilterDataLogger((DateTime)Logger_DatePicker_1.SelectedDate, (DateTime)Logger_DatePicker_2.SelectedDate, DataGrid_Logger_Filter_TextBox.Text);
                DataGrid_Logger.ItemsSource = LoggerList;
                DataGrid_Logger_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
        }

        private void Logger_DatePicker_1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Logger_DatePicker_2.DisplayDateStart = Logger_DatePicker_1.SelectedDate;
            try
            {

                LoggerList = dataManagement.SearchFilterDataLogger((DateTime)Logger_DatePicker_1.SelectedDate, (DateTime)Logger_DatePicker_2.SelectedDate, DataGrid_Logger_Filter_TextBox.Text);
                DataGrid_Logger.ItemsSource = LoggerList;
                DataGrid_Logger_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
        }

        

        public void DataGrid_Logger_SubItem_Add()
        {
            //Logger_WriteLine("[DataGrid_Logger_SubItem_Add() 시작]");

            try
            {
                LoggerList = dataManagement.SearchFilterDataLogger((DateTime)Logger_DatePicker_1.SelectedDate, (DateTime)Logger_DatePicker_2.SelectedDate, DataGrid_Logger_Filter_TextBox.Text);
                DataGrid_Logger.ItemsSource = LoggerList;

                DataGrid_Logger.SelectedIndex = DataGrid_Logger.Items.Count - 1;
                DataGrid_Logger_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }
            //Logger_WriteLine("[DataGrid_Logger_SubItem_Add() 종료]");
        }

        

        public void DataGrid_Logger_SubItem_Add(String Comment)
        {
            //Logger_WriteLine("[DataGrid_Logger_SubItem_Add() 시작]");

            try
            {
                LoggerList list = new LoggerList(dataManagement.NewIndexNum("logger_list"), DateTime.Now, Comment);
                //LoggerList.Add(list); 
                
                //dataManagement.InsertDataLogger(LoggerList[LoggerList.Count - 1]);
                //DataGrid_Logger.ItemsSource = null;
                //DataGrid_Logger.ItemsSource = LoggerList;

                //DataGrid_Logger.SelectedIndex = DataGrid_Logger.Items.Count - 1;
                //DataGrid_Logger.ScrollIntoView(LoggerList[LoggerList.Count - 1]);
                //DataGrid_Logger_Data_Changed();
            }
            catch (Exception ex)
            {
                //Logger_WriteLine(ex.Message);
            }
            //Logger_WriteLine("[DataGrid_Logger_SubItem_Add() 종료]");
        }

        private void DataGrid_Logger_Data_Changed()
        {
            DataGrid_LoggerList_ItemCount_txtb.Text = LoggerList.Count + " Data(s) find";
        }

 

        private void DataGrid_Logger_Filter_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                
                LoggerList = dataManagement.SearchFilterDataLogger((DateTime)Logger_DatePicker_1.SelectedDate, (DateTime)Logger_DatePicker_2.SelectedDate, DataGrid_Logger_Filter_TextBox.Text);
                DataGrid_Logger.ItemsSource = LoggerList;
                DataGrid_Logger_Data_Changed();
            }
            catch (Exception ex)
            {
                Logger_WriteLine(ex.Message);
            }

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.google.com/search?q=" + DataGrid_Part_Filter_TextBox.Text);
        }

        #endregion

        #region Logger 관련 함수

        public void Logger_WriteLine(String message)
        {
            DataGrid_Logger_SubItem_Add(message);
            Logger_txtb.Text += Environment.NewLine + message;
            //Logger_txtb.SelectionStart = Logger_txtb.Text.Length;//맨 마지막 선택...
            Logger_txtb.ScrollToEnd();
        }


        #endregion

        #region 설정 관련 함수


        // 부품 카테고리 변경 함수
        private void button_Click(object sender, RoutedEventArgs e)
        {
            int a = Convert.ToInt32(label4.Content);
            for (int i=0;i< PartList.Count;i++)
            {
                if(PartList[i].Category == textBox.Text)
                {
                    PartList[i].Category = textBox1.Text;
                    Logger_WriteLine(dataManagement.UpdateDataPart(PartList[i]));
                    a++;
                    label4.Content = a.ToString();
                }
            }

            DataGrid_Part.ItemsSource = null;
            DataGrid_Part.ItemsSource = PartList;
            
        }

        #endregion


        

    }



}

