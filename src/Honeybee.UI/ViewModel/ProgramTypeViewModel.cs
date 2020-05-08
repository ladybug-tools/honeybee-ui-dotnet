using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Honeybee.UI
{
    //public class ProgramTypeViewModel : INotifyPropertyChanged
    //{

    //    private double _peopleArea;
    //    public double PeopleArea
    //    {
    //        get { return _peopleArea; }
    //        set
    //        {
    //            if (_peopleArea != value)
    //            {
    //                //MessageBox.Show("isoutdoor: " + value);
    //                _peopleArea = value;
    //                OnPropertyChanged();
    //            }
    //        }

    //    }

    //    private static ProgramTypeViewModel _instance;
    //    public static ProgramTypeViewModel Instance
    //    {
    //        get
    //        {
    //            if (_instance == null)
    //            {
    //                _instance = new ProgramTypeViewModel();
    //            }
    //            return _instance;
    //        }
    //    }

    //    private ProgramTypeViewModel()
    //    {
    //        this.PeopleArea = 0.1;
    //    }


    //    void OnPropertyChanged([CallerMemberName] string memberName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
    //    }
    //    public event PropertyChangedEventHandler PropertyChanged;
    //}

    public class ProgramTypeViewModel : ViewModelBase
    {
        private ProgramTypeAbridged _hbObj;
        public ProgramTypeAbridged hbObj
        {
            get => _hbObj;
            set => Set(() => _hbObj = value, nameof(hbObj));
        }

        public double PeoplePerArea
        {
            get => _hbObj.People.PeoplePerArea;
            set => Set(() => _hbObj.People.PeoplePerArea = value, nameof(PeoplePerArea));
        }
        public string OccupancySchedule
        {
            get => _hbObj.People.OccupancySchedule;
            set => Set(() => _hbObj.People.OccupancySchedule = value, nameof(OccupancySchedule));
        }
        public string ActivitySchedule
        {
            get => _hbObj.People.ActivitySchedule;
            set => Set(() => _hbObj.People.ActivitySchedule = value, nameof(ActivitySchedule));
        }
        public double RadiantFraction
        {
            get => _hbObj.People.RadiantFraction;
            set => Set(() => _hbObj.People.RadiantFraction = value, nameof(RadiantFraction));
        }
        public double LatentFraction
        {
            get => _hbObj.People.LatentFraction.Obj is double? Double.Parse( _hbObj.People.LatentFraction.ToString()) : 0;
            set => Set(() => _hbObj.People.LatentFraction = value, nameof(LatentFraction));
        }

        public bool IsLatentFractionAutocalculate
        {
            get => _hbObj.People.LatentFraction.Obj is Autocalculate;
            set => Set(
                () => {
                    if (value)
                        _hbObj.People.LatentFraction = new Autocalculate();
                }, 
                nameof(IsLatentFractionAutocalculate));
        }

        private static ProgramTypeViewModel _instance;
        public static ProgramTypeViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProgramTypeViewModel();
                }
                return _instance;
            }
        }

        public void UpdatePeople(PeopleAbridged newPeople)
        {
            this.hbObj.People = newPeople;

            //this.PeoplePerArea = newPeople.PeoplePerArea;
            //this.OccupancySchedule = newPeople.OccupancySchedule;
            //this.ActivitySchedule = newPeople.ActivitySchedule;
            //this.RadiantFraction = newPeople.RadiantFraction;
            //if (newPeople.LatentFraction.Obj is double num)
            //{
            //    this.LatentFraction = num;
            //    this.IsLatentFractionAutocalculate = false;
            //}
            //else
            //{
            //    this.LatentFraction = 0;
            //    this.IsLatentFractionAutocalculate = true;
            //}
            var propertiesTobeRefreshed = new List<string>()
            {
                nameof(this.PeoplePerArea),
                nameof(this.OccupancySchedule),
                nameof(this.ActivitySchedule),
                nameof(this.RadiantFraction),
                nameof(this.LatentFraction),
                nameof(this.IsLatentFractionAutocalculate)
            };
            this.RefreshControls(propertiesTobeRefreshed);
         
        }

        private ProgramTypeViewModel()
        {
        }

    }



}
