using System;
using System.ComponentModel;

namespace gestion_etudiant.Models
{
    public class Etudiant : INotifyPropertyChanged
    {
        private int _id;
        private string _nom;
        private string _prenom;
        private string _email;
        private DateTime? _dateNaissance;
        private string _telephone;
        private string _adresse;
        private DateTime _dateCreation;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Nom
        {
            get { return _nom; }
            set
            {
                _nom = value;
                OnPropertyChanged("Nom");
            }
        }

        public string Prenom
        {
            get { return _prenom; }
            set
            {
                _prenom = value;
                OnPropertyChanged("Prenom");
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }

        public DateTime? DateNaissance
        {
            get { return _dateNaissance; }
            set
            {
                _dateNaissance = value;
                OnPropertyChanged("DateNaissance");
            }
        }

        public string Telephone
        {
            get { return _telephone; }
            set
            {
                _telephone = value;
                OnPropertyChanged("Telephone");
            }
        }

        public string Adresse
        {
            get { return _adresse; }
            set
            {
                _adresse = value;
                OnPropertyChanged("Adresse");
            }
        }

        public DateTime DateCreation
        {
            get { return _dateCreation; }
            set
            {
                _dateCreation = value;
                OnPropertyChanged("DateCreation");
            }
        }

        public string NomComplet
        {
            get { return $"{Prenom} {Nom}"; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}