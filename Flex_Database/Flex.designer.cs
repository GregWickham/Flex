﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Flex.Database
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Flex")]
	public partial class FlexDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertDB_WeightedWord(DB_WeightedWord instance);
    partial void UpdateDB_WeightedWord(DB_WeightedWord instance);
    partial void DeleteDB_WeightedWord(DB_WeightedWord instance);
    partial void InsertDB_WordBuilder(DB_WordBuilder instance);
    partial void UpdateDB_WordBuilder(DB_WordBuilder instance);
    partial void DeleteDB_WordBuilder(DB_WordBuilder instance);
    #endregion
		
		public FlexDataContext() : 
				base(global::Flex.Database.Properties.Settings.Default.FlexConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public FlexDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public FlexDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public FlexDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public FlexDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<DB_WeightedWord> DB_WeightedWords
		{
			get
			{
				return this.GetTable<DB_WeightedWord>();
			}
		}
		
		public System.Data.Linq.Table<DB_WordBuilder> DB_WordBuilders
		{
			get
			{
				return this.GetTable<DB_WordBuilder>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.DB_WeightedWords")]
	public partial class DB_WeightedWord : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private string _Word;
		
		private System.Nullable<int> _Weight;
		
		private System.Nullable<int> _Builder;
		
		private EntitySet<DB_WordBuilder> _DB_WordBuilders;
		
		private EntityRef<DB_WordBuilder> _DB_WordBuilder;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnWordChanging(string value);
    partial void OnWordChanged();
    partial void OnWeightChanging(System.Nullable<int> value);
    partial void OnWeightChanged();
    partial void OnBuilderChanging(System.Nullable<int> value);
    partial void OnBuilderChanged();
    #endregion
		
		public DB_WeightedWord()
		{
			this._DB_WordBuilders = new EntitySet<DB_WordBuilder>(new Action<DB_WordBuilder>(this.attach_DB_WordBuilders), new Action<DB_WordBuilder>(this.detach_DB_WordBuilders));
			this._DB_WordBuilder = default(EntityRef<DB_WordBuilder>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Word", DbType="VarChar(24) NOT NULL", CanBeNull=false)]
		public string Word
		{
			get
			{
				return this._Word;
			}
			set
			{
				if ((this._Word != value))
				{
					this.OnWordChanging(value);
					this.SendPropertyChanging();
					this._Word = value;
					this.SendPropertyChanged("Word");
					this.OnWordChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Weight", DbType="Int")]
		public System.Nullable<int> Weight
		{
			get
			{
				return this._Weight;
			}
			set
			{
				if ((this._Weight != value))
				{
					this.OnWeightChanging(value);
					this.SendPropertyChanging();
					this._Weight = value;
					this.SendPropertyChanged("Weight");
					this.OnWeightChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Builder", DbType="Int")]
		public System.Nullable<int> Builder
		{
			get
			{
				return this._Builder;
			}
			set
			{
				if ((this._Builder != value))
				{
					if (this._DB_WordBuilder.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnBuilderChanging(value);
					this.SendPropertyChanging();
					this._Builder = value;
					this.SendPropertyChanged("Builder");
					this.OnBuilderChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="DB_WeightedWord_DB_WordBuilder", Storage="_DB_WordBuilders", ThisKey="ID", OtherKey="DefaultForm")]
		public EntitySet<DB_WordBuilder> DB_WordBuilders
		{
			get
			{
				return this._DB_WordBuilders;
			}
			set
			{
				this._DB_WordBuilders.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="DB_WordBuilder_DB_WeightedWord", Storage="_DB_WordBuilder", ThisKey="Builder", OtherKey="ID", IsForeignKey=true)]
		public DB_WordBuilder DB_WordBuilder
		{
			get
			{
				return this._DB_WordBuilder.Entity;
			}
			set
			{
				DB_WordBuilder previousValue = this._DB_WordBuilder.Entity;
				if (((previousValue != value) 
							|| (this._DB_WordBuilder.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._DB_WordBuilder.Entity = null;
						previousValue.DB_WeightedWords.Remove(this);
					}
					this._DB_WordBuilder.Entity = value;
					if ((value != null))
					{
						value.DB_WeightedWords.Add(this);
						this._Builder = value.ID;
					}
					else
					{
						this._Builder = default(Nullable<int>);
					}
					this.SendPropertyChanged("DB_WordBuilder");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_DB_WordBuilders(DB_WordBuilder entity)
		{
			this.SendPropertyChanging();
			entity.DB_WeightedWord = this;
		}
		
		private void detach_DB_WordBuilders(DB_WordBuilder entity)
		{
			this.SendPropertyChanging();
			entity.DB_WeightedWord = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.DB_WordBuilders")]
	public partial class DB_WordBuilder : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private System.Nullable<int> _DefaultForm;
		
		private string _SingleWord;
		
		private EntitySet<DB_WeightedWord> _DB_WeightedWords;
		
		private EntityRef<DB_WeightedWord> _DB_WeightedWord;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnDefaultFormChanging(System.Nullable<int> value);
    partial void OnDefaultFormChanged();
    partial void OnSingleWordChanging(string value);
    partial void OnSingleWordChanged();
    #endregion
		
		public DB_WordBuilder()
		{
			this._DB_WeightedWords = new EntitySet<DB_WeightedWord>(new Action<DB_WeightedWord>(this.attach_DB_WeightedWords), new Action<DB_WeightedWord>(this.detach_DB_WeightedWords));
			this._DB_WeightedWord = default(EntityRef<DB_WeightedWord>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DefaultForm", DbType="Int")]
		public System.Nullable<int> DefaultForm
		{
			get
			{
				return this._DefaultForm;
			}
			set
			{
				if ((this._DefaultForm != value))
				{
					if (this._DB_WeightedWord.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnDefaultFormChanging(value);
					this.SendPropertyChanging();
					this._DefaultForm = value;
					this.SendPropertyChanged("DefaultForm");
					this.OnDefaultFormChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SingleWord", DbType="VarChar(24)")]
		public string SingleWord
		{
			get
			{
				return this._SingleWord;
			}
			set
			{
				if ((this._SingleWord != value))
				{
					this.OnSingleWordChanging(value);
					this.SendPropertyChanging();
					this._SingleWord = value;
					this.SendPropertyChanged("SingleWord");
					this.OnSingleWordChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="DB_WordBuilder_DB_WeightedWord", Storage="_DB_WeightedWords", ThisKey="ID", OtherKey="Builder")]
		public EntitySet<DB_WeightedWord> DB_WeightedWords
		{
			get
			{
				return this._DB_WeightedWords;
			}
			set
			{
				this._DB_WeightedWords.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="DB_WeightedWord_DB_WordBuilder", Storage="_DB_WeightedWord", ThisKey="DefaultForm", OtherKey="ID", IsForeignKey=true)]
		public DB_WeightedWord DB_WeightedWord
		{
			get
			{
				return this._DB_WeightedWord.Entity;
			}
			set
			{
				DB_WeightedWord previousValue = this._DB_WeightedWord.Entity;
				if (((previousValue != value) 
							|| (this._DB_WeightedWord.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._DB_WeightedWord.Entity = null;
						previousValue.DB_WordBuilders.Remove(this);
					}
					this._DB_WeightedWord.Entity = value;
					if ((value != null))
					{
						value.DB_WordBuilders.Add(this);
						this._DefaultForm = value.ID;
					}
					else
					{
						this._DefaultForm = default(Nullable<int>);
					}
					this.SendPropertyChanged("DB_WeightedWord");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_DB_WeightedWords(DB_WeightedWord entity)
		{
			this.SendPropertyChanging();
			entity.DB_WordBuilder = this;
		}
		
		private void detach_DB_WeightedWords(DB_WeightedWord entity)
		{
			this.SendPropertyChanging();
			entity.DB_WordBuilder = null;
		}
	}
}
#pragma warning restore 1591