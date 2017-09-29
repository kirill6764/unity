using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class MainScript : MonoBehaviour {

	public InputField user_name;
	public InputField user_secondName;
	public InputField user_email;
    public Text errorsText;
	private string path;
    // Use this for initialization
    void Start () {
		path = Application.persistentDataPath + "/";
		string pathSCV = path + "save.csv";
		if (!File.Exists(pathSCV))
        {
            //File.Create(path);
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
			SendMail(user_email.text);
		}
    }

	public void CreatePhoto(){
		SceneManager.LoadScene ("Camera");
	}

	private void SendMail(string Email)
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
		string to = Email;
		//Тема письма
		string subject = "Тестовое приложение. Письмо";
		//Текст письма
		string body = "Привет, " + user_name.text + "! \n\n Ваше фото.";

		//Создаем сообщение
		MailMessage mess = new MailMessage(from, to, subject, body);

		if (File.Exists (pathPhoto)) {
			Attachment attach = new Attachment (pathPhoto);
			mess.Attachments.Add (attach);
		} else {
			errorsText.text = "Необходимо сделать фото";
			return;
		}
			
		try{
			client.Send(mess);
			errorsText.text = "Сообщение отправлено";
		}catch(System.Exception ex){
			errorsText.text = "Ошибка при отправке сообщения";
		}
	}
	public static bool isValidMail(string email)
	{
		string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
		Match isMatch = Regex.Match(email, pattern, RegexOptions.IgnoreCase);
		return isMatch.Success;
	}

	// Update is called once per frame
	void Update () {
		if (Application.platform == RuntimePlatform.Android)
		{
			if (Input.GetKeyDown(KeyCode.Escape)) 
			{
				Application.Quit(); 
			}
		}
	}
}
