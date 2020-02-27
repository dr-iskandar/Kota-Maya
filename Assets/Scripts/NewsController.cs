using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;

public class NewsController : MonoBehaviour
{
    public string jsontext;
    public string[] news;
    public string[] newsTitle;
    public string[] newsDetail;
    public Sprite[] newsImage;
    public int[] index;
    public Button[] DetailsButtons;

    public GameObject newsScroller, newsButton;
    public string tes;
    public GameObject Detail;

    private IEnumerator Start()
    {
        WWWForm form = new WWWForm();
        form.AddField("limit", "0");
        form.AddField("offset", "0");

        using (UnityWebRequest www = UnityWebRequest.Post("https://api.open-aryanna.com/news/getAllNews", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                jsontext = www.downloadHandler.text;
                StartCoroutine(parseData());
            }
        }
    }

    IEnumerator parseData()
    {
        var N = JSON.Parse(jsontext);
        news = new string[N["data"].Count];
        newsImage = new Sprite[N["data"].Count];
        newsTitle = new string[N["data"].Count];
        newsDetail = new string[N["data"].Count];
        index = new int [N["data"].Count];
        
        for (int i = 0; i < news.Length; i++)
        {
            news[i] = N["data"][i].ToString();
            newsTitle[i] = N["data"][i]["title"].Value;
            newsDetail[i] = N["data"][i]["content"].Value;
            index[i] = int.Parse(N["data"][i]["newsId"].Value);

            var buttonNews = Instantiate(newsButton);
            buttonNews.transform.SetParent(newsScroller.transform);
            buttonNews.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            yield return new WaitForEndOfFrame();
            WWW www = new WWW(N["data"][i]["imageUrl"].Value);
            yield return www;
            newsImage[i] = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            buttonNews.transform.GetChild(0).GetComponent<Image>().sprite = newsImage[i];
            buttonNews.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = N["data"][i]["title"].Value;
            int closureIndex = i;
            buttonNews.GetComponent<Button>().onClick.AddListener(() => DetailNews(closureIndex));
            //DetailNews(i);
        }
    }

    public void DetailNews(int Index)
    {
        Detail.SetActive(true);
        Detail.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = newsTitle[Index];
        Detail.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = newsImage[Index];
        Detail.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = newsDetail[Index].Replace("<p>","").Replace("</p>","").Replace("<br />"," <br>");
    }
}

