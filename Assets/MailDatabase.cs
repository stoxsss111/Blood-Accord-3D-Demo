using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MailDatabase", menuName = "MiniPC/MailDatabase")]
public class MailDatabase : ScriptableObject
{
    [System.Serializable]
    public class Mail
    {
        public string sender;
        [TextArea] public string body;
        public bool hasAttachment;
        public string attachmentName;
    }

    public List<Mail> mails;
    public string mainPassword;
}