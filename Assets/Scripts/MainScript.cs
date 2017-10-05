using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Threading;

public class MainScript : MonoBehaviour {

	public  InputField user_name;
	public  InputField user_secondName;
	public InputField user_email;
	public static MainScript instance;

	public Text errorsText;

	private static bool sendMail;
	private string path;

	void Awake(){
		if (!instance) {
			instance = this;
			DontDestroyOnLoad (transform.gameObject);
		} else {
			Destroy(gameObject);
		}
	}
	void OnLevelWasLoaded(int level){
		
		Debug.Log ("Load Scene");
	}

    void Start () {
		sendMail = false;
		path = Application.persistentDataPath + "/";
		string pathSCV = path + "save.csv";
		if (!File.Exists(pathSCV))
        {
			StreamWriter csv = new StreamWriter(pathSCV, true, System.Text.Encoding.UTF8);
            csv.WriteLine("Имя;Фамилия;Почта"); // это заголовок. разделитель ; может быть другим
            csv.Close();
        }
    }

	

	public void MyOnCLickButton(){
		string pathSCV = path + "save.csv";
        //string path = Application.persistentDataPath + "/save.csv";

        List<string> errors = new List<string>();
		errorsText.text = "";
		if (user_name.text == "") {
			errors.Add("Не заполнено имя");
		}
		if (user_secondName.text == "") {
			errors.Add("Не заполнена фамилия");
		}
		if (user_email.text == "") {
			errors.Add("Не заполнена почта");
		}else if(!isValidMail(user_email.text)){
			errors.Add("Email не корректен");
		}
		if (errors.Count > 0) {
			errorsText.text = errors [0];
		} else {
			errorsText.text = "Тестовое задание";
			StreamWriter csv = new StreamWriter(pathSCV, true, System.Text.Encoding.UTF8);
			csv.WriteLine(user_name.text + ";" + user_secondName.text + ";" + user_email.text); // это заголовок. разделитель ; может быть другим
			csv.Close();
			if (File.Exists (path + "avatar.png")) {
				ThreadStart threadStart = new ThreadStart(SendMail);
				Thread thread = new Thread (threadStart);
				thread.IsBackground = true;
				thread.Start();
			} else {
				errorsText.text = "Необходимо сделать фото";
				return;
			}
		}
    }

	public void CreatePhoto(){
		SceneManager.LoadScene ("Camera");
	}

	private void SendMail()
	{
		string pathPhoto = path + "avatar.png";
		//smtp сервер
		string smtpHost = "smtp.timeweb.ru";
		//smtp порт
		int smtpPort = 2525;
		//логин
		string login = "kirill@klibr.ru";
		//пароль
		string pass = "XM75k9nE";
		 
		//создаем подключение
		SmtpClient client = new SmtpClient(smtpHost, smtpPort);

		client.Credentials = ( (System.Net.ICredentialsByHost) (new NetworkCredential(login, pass)));

		//От кого письмо
		string from = "kirill@klibr.ru";
		//Кому письмо
		string to = user_email.text;
		//Тема письма
		string subject = "Тестовое приложение. Письмо";
		//Текст письма
		string body = "Привет, " + user_name.text + "! \n\n Ваше фото.";

		//Создаем сообщение
		MailMessage mess = new MailMessage(from, to, subject, body);
		Attachment attach = new Attachment (pathPhoto);
		mess.Attachments.Add (attach);
			
		try{
			client.Send(mess);
			sendMail = true;
		}catch(System.Exception ex){
		}
	}
	public bool isValidMail(string email)
	{
		string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
		Match isMatch = Regex.Match(email, pattern, RegexOptions.IgnoreCase);
		return isMatch.Success;
	}

	// Update is called once per frame
	void Update () {
		if (sendMail) {
			errorsText.text = "Сообщение отправлено";
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			if (Input.GetKeyDown(KeyCode.Escape)) 
			{
				Application.Quit(); 
			}
		}
	}
}
