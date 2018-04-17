using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

 public class PlayFabLogin : MonoBehaviour
    {
        LoginResult m_result;
        static string m_StoreID = "SouthAfrica"; //name of your store
        public void Start()
        {
            //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
            PlayFabSettings.TitleId = "FA60"; // Please change this value to your own titleId from PlayFab Game Manager

            var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true};
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }

        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("Congratulations, you made your first successful API call!");
            Debug.Log(result);
            m_result = result;
            GetTitleData();
            GetStatistics();
            GetCatalogItems();
            GetStoreItems();
            ExecuteCloudScript();
        }

        private void OnLoginFailure(PlayFabError error)
        {
            Debug.LogWarning("Something went wrong with your first API call.  :(");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }
        private void GetStatistics(){
            GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest{StatisticNames = new List<string>(new string[]{"xp"})};
            PlayFabClientAPI.GetPlayerStatistics(request, OnGetStatisticsSuccess, OnGetStatisticsError);
        }
        private void OnGetStatisticsSuccess(GetPlayerStatisticsResult result){
            Debug.Log("Successfully got statistics!");
            foreach(StatisticValue s in result.Statistics){
                Debug.Log(s.StatisticName + ", " + s.Value + ", version: " + s.Version);
            }
        }
        private void OnGetStatisticsError(PlayFabError error){
            Debug.LogWarning("Something spooky happened when trying to get statstics");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }
        private void GetTitleData(){
            GetTitleDataRequest request = new GetTitleDataRequest();
            PlayFabClientAPI.GetTitleData(request, OnGetTitleDataSuccess, OnGetTitleDataError);
        }
        private void OnGetTitleDataSuccess(GetTitleDataResult result){
            Debug.Log("Successfully got title data!");
            foreach(KeyValuePair<string, string> kvp in result.Data){
                Debug.Log(kvp.Key + ": " + kvp.Value);
            }
        }
        private void OnGetTitleDataError(PlayFabError error){
            Debug.LogWarning("Something spooky happened when trying to get title data");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }

        private void GetCatalogItems(){
            GetCatalogItemsRequest request = new GetCatalogItemsRequest();
            PlayFabClientAPI.GetCatalogItems(request, OnGetCatalogItemsSuccess, OnGetCatalogItemsFailure);
        }
        private void OnGetCatalogItemsSuccess(GetCatalogItemsResult result){
            Debug.Log("Successfully got catalogue items!");
            foreach(CatalogItem item in result.Catalog){
                PurchaseItem(item);
            }
        }
        private void OnGetCatalogItemsFailure(PlayFabError error){
            Debug.LogWarning("Something spooky happened when trying to get catalogue items");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }

        private void PurchaseItem(CatalogItem item){
            Debug.Log("Buying: " + item.ItemId);
            KeyValuePair<string, uint> price;
            if(item.VirtualCurrencyPrices.Count > 0){
                price = item.VirtualCurrencyPrices.ToList<KeyValuePair<string, uint>>()[0];
            }
            else{
                price = new KeyValuePair<string, uint>("", 0);
            }
            Debug.Log("Price: " + price.Key + ": " + price.Value);
            PurchaseItemRequest request = new PurchaseItemRequest{
                CatalogVersion = item.CatalogVersion, 
                ItemId = item.ItemId, 
                VirtualCurrency = price.Key,
                Price = (int)price.Value
            };
            PlayFabClientAPI.PurchaseItem(request, OnPurchaseItemSuccess, OnPurchaseItemFailure);
        }
        private void OnPurchaseItemSuccess(PurchaseItemResult result){
            Debug.Log("Successfullt bought the item!");
            foreach(ItemInstance item in result.Items){
                Debug.Log(item.DisplayName + ": {ID: " + item.ItemId + ", InstanceID: " + item.ItemInstanceId + "}");
            }
        }
        private void OnPurchaseItemFailure(PlayFabError error){
            Debug.LogWarning("Something spooky happened when trying to purchase");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }

        private void GetStoreItems(){
            GetStoreItemsRequest request = new GetStoreItemsRequest{StoreId = m_StoreID};
            PlayFabClientAPI.GetStoreItems(request, OnGetStoreItemsSuccess, OnGetStoreItemsFailure);
        }
        private void OnGetStoreItemsSuccess(GetStoreItemsResult result){
            Debug.Log("Successfully got store items!");
            foreach(StoreItem item in result.Store){
                PurchaseStoreItem(item, result.StoreId);
            }
        }
        private void OnGetStoreItemsFailure(PlayFabError error){
            Debug.LogWarning("Something spooky happened when trying to get store items");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }
        private void PurchaseStoreItem(StoreItem item, string StoreID){
            Debug.Log("Buying (store): " + item.ItemId);
            KeyValuePair<string, uint> price;
            if(item.VirtualCurrencyPrices.Count > 0){
                price = item.VirtualCurrencyPrices.ToList<KeyValuePair<string, uint>>()[0];
            }
            else{
                price = new KeyValuePair<string, uint>("", 0);
            }
            Debug.Log("Price: " + price.Key + ": " + price.Value);
            PurchaseItemRequest request = new PurchaseItemRequest{
                StoreId = StoreID,
                ItemId = item.ItemId, 
                VirtualCurrency = price.Key,
                Price = (int)price.Value
            };
            PlayFabClientAPI.PurchaseItem(request, OnPurchaseItemSuccess, OnPurchaseItemFailure);
        }
        private void ExecuteCloudScript(){
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest{FunctionName = "bushelOnYourFirstDay"};
            PlayFabClientAPI.ExecuteCloudScript(request, OnExecuteCloudScriptSuccess, OnExecuteCloudScriptFailure);
        }
        void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult result){
            Debug.Log("Successfully executed cloud scripts!");
            foreach(LogStatement log in result.Logs){
                Debug.Log(log.Message);
            }
        }
        void OnExecuteCloudScriptFailure(PlayFabError error){
            Debug.LogWarning("Something spooky happened when trying to execute cloud scripts");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }
    }