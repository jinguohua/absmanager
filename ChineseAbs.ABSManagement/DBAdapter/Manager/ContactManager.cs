using ABSMgrConn;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;

namespace ChineseAbs.ABSManagement
{
    public class ContactManager : BaseManager
    {
        public ContactManager() { }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerContact(UserInfo);
        }

        #region Methods

        public Contacts GetContacts()
        {
            var tblContacts = m_db.Query<TableContacts>
                ("select * from dbo.Contacts");
            Contacts rt = new Contacts();
            foreach (var item in tblContacts)
            {
                Contact contact = new Contact(item);
                contact = Decrypt(contact);
                rt.Add(contact);
            }
            return rt;
        }

        public Contacts GetContacts(int projectId) 
        {
            var tblContacts = m_db.Query<TableContacts>
                ("select * from dbo.Contacts where project_id = @0 ",projectId);
            Contacts rt = new Contacts();
            foreach (var item in tblContacts)
            {
                Contact contact = new Contact(item);
                contact = Decrypt(contact);
                rt.Add(contact);
            }
            return rt;
        }

        public void UpdateContact(Contact contact)
        {
            contact.TimeStamp = DateTime.Now;
            contact = Encrypt(contact);
            m_db.Update("dbo.Contacts", "contact_id", contact.GetTableObject(), contact.ContactId);
            m_logger.Log(contact.ProjectId,"更新联系人");
        }

        public int AddContact(Contact contact)
        {
            contact.Guid = Guid.NewGuid().ToString();
            contact.TimeStamp = DateTime.Now;
            contact = Encrypt(contact);
            m_logger.Log(contact.ProjectId, "新增联系人");
            return (int)m_db.Insert(contact.GetTableObject());
        }

        public void RemoveContact(int contactId,int projectId)
        {
            m_logger.Log(projectId, "删除联系人");
            m_db.Delete<TableContacts>("where contact_id = @0 ", contactId);
        }

        public int GetMaxAccountId()
        {
            int id = m_db.Single<int>(" select ident_current('dbo.Contacts')");
            return id;
        }

        public bool Exists(string contactGuid)
        {
            return Exists<ABSMgrConn.TableContacts>("dbo.Contacts", "contact_guid", contactGuid);
        }

        public Contact GetByGuid(string contactGuid)
        {
            var record = SelectSingle<TableContacts>("dbo.Contacts", "contact_guid", contactGuid);
            var contact = new Contact(record);
            return Decrypt(contact);
        }


        public Contact GetById(int contactId)
        {
            var contact = SelectSingle<TableContacts>("dbo.Contacts", "contact_id", contactId);
            return new Contact(contact);
        }

        public Contacts GetContactsByProjectId(int pid)
        {
            var contactsIE = m_db.Query<TableContacts>("select * from dbo.Contacts where project_id =@0", pid);
            Contacts rt = new Contacts();
            foreach (var item in contactsIE)
            {
                Contact contact = new Contact(item);
                contact = Decrypt(contact);
                rt.Add(contact);
            }
            return rt;
        }

        #endregion


        private Contact Encrypt(Contact contact)
        {
            contact.Name = RsaUtils.Encrypt(contact.Name);
            contact.Email = RsaUtils.Encrypt(contact.Email);
            contact.CellPhone = RsaUtils.Encrypt(contact.CellPhone);
            return contact;
        }

        private Contact Decrypt(Contact contact)
        {
            contact.Name = RsaUtils.Decrypt<string>(contact.Name);
            contact.Email = RsaUtils.Decrypt<string>(contact.Email);
            contact.CellPhone = RsaUtils.Decrypt<string>(contact.CellPhone); ;
            return contact;
        }

    }
}
