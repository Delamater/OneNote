﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using OneNoteServiceSamplesWinUniversal.OneNoteApi.Notebooks;
using OneNoteServiceSamplesWinUniversal.OneNoteApi.Pages;
using OneNoteServiceSamplesWinUniversal.OneNoteApi.SectionGroups;
using OneNoteServiceSamplesWinUniversal.OneNoteApi.Sections;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace OneNoteServiceSamplesWinUniversal.Data
{
	/// <summary>
	/// Generic item data model.
	/// </summary>
	public class SampleDataItem
	{
		public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description,
			bool requiresInputComboBox1, bool requiresInputComboBox2, bool requiresInputTextBox)
		{
			this.UniqueId = uniqueId;
			this.Title = title;
			this.Subtitle = subtitle;
			this.Description = description;
			this.ImagePath = imagePath;
			this.RequiresInputComboBox1 = requiresInputComboBox1;
			this.RequiresInputComboBox2 = requiresInputComboBox2;
			this.RequiresInputTextBox = requiresInputTextBox;
		}

		public string UniqueId { get; private set; }
		public string Title { get; private set; }
		public string Subtitle { get; private set; }
		public string Description { get; private set; }
		public string ImagePath { get; private set; }

		public bool RequiresInputComboBox1 { get; private set; }
		public bool RequiresInputComboBox2 { get; private set; }
		public bool RequiresInputTextBox { get; private set; }
		public override string ToString()
		{
			return this.Title;
		}
	}

	/// <summary>
	/// Generic group data model.
	/// </summary>
	public class SampleDataGroup
	{
		public SampleDataGroup(String uniqueId, String title, String imagePath, String description)
		{
			this.UniqueId = uniqueId;
			this.Title = title;
			this.Description = description;
			this.ImagePath = imagePath;
			this.Items = new ObservableCollection<SampleDataItem>();
		}

		public string UniqueId { get; private set; }
		public string Title { get; private set; }
		public string Description { get; private set; }
		public string ImagePath { get; private set; }
		public ObservableCollection<SampleDataItem> Items { get; private set; }

		public override string ToString()
		{
			return this.Title;
		}
	}

	/// <summary>
	/// Creates a collection of groups and items with content read from a static json file.
	/// 
	/// SampleDataSource initializes with data read from a static json file included in the 
	/// project.  This provides sample data at both design-time and run-time.
	/// </summary>
	public sealed class SampleDataSource
	{
		private static SampleDataSource _sampleDataSource = new SampleDataSource();

		private ObservableCollection<SampleDataGroup> _groups = new ObservableCollection<SampleDataGroup>();
		public ObservableCollection<SampleDataGroup> Groups
		{
			get { return this._groups; }
		}

		public static async Task<IEnumerable<SampleDataGroup>> GetGroupsAsync()
		{
			await _sampleDataSource.GetSampleDataAsync();

			return _sampleDataSource.Groups;
		}

		public static async Task<SampleDataGroup> GetGroupAsync(string uniqueId)
		{
			await _sampleDataSource.GetSampleDataAsync();
			// Simple linear search is acceptable for small data sets
			var matches = _sampleDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
			if (matches.Count() == 1) return matches.First();
			return null;
		}

		public static async Task<SampleDataItem> GetItemAsync(string uniqueId)
		{
			await _sampleDataSource.GetSampleDataAsync();
			// Simple linear search is acceptable for small data sets
			var matches = _sampleDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
			if (matches.Count() == 1) return matches.First();
			return null;
		}

		private async Task GetSampleDataAsync()
		{
			if (this._groups.Count != 0)
				return;

			Uri dataUri = new Uri("ms-appx:///DataModel/SampleData.json");

			StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
			string jsonText = await FileIO.ReadTextAsync(file);
			JsonObject jsonObject = JsonObject.Parse(jsonText);
			JsonArray jsonArray = jsonObject["Groups"].GetArray();

			foreach (JsonValue groupValue in jsonArray)
			{
				JsonObject groupObject = groupValue.GetObject();
				SampleDataGroup group = new SampleDataGroup(groupObject["UniqueId"].GetString(),
															groupObject["Title"].GetString(),
															groupObject["ImagePath"].GetString(),
															groupObject["Description"].GetString());

				foreach (JsonValue itemValue in groupObject["Items"].GetArray())
				{
					JsonObject itemObject = itemValue.GetObject();
					IJsonValue subtitleValue;
					itemObject.TryGetValue("Subtitle", out subtitleValue);
					string subtitle = (subtitleValue == null) ? "" : subtitleValue.GetString();
					group.Items.Add(new SampleDataItem(itemObject["UniqueId"].GetString(),
													   itemObject["Title"].GetString(),
													   subtitle,
													   itemObject["ImagePath"].GetString(),
													   itemObject["Description"].GetString(),
													   Convert.ToBoolean(itemObject["RequiresInputComboBox1"].GetString()),
													   Convert.ToBoolean(itemObject["RequiresInputComboBox2"].GetString()),
													   Convert.ToBoolean(itemObject["RequiresInputTextBox"].GetString())));
				}
				this.Groups.Add(group);
			}
		}

		public static async Task<object> ExecuteApi(string uniqueId, bool debug, string requiredSelectedId, string requiredInputText)
		{
			switch (uniqueId)
			{
				case "Group-0-Item-0":
				   return await PostPagesExample.CreateSimplePage(debug);
				case "Group-0-Item-1":
					return await PostPagesExample.CreatePageWithImage(debug);
				case "Group-0-Item-2":
					return await PostPagesExample.CreatePageWithEmbeddedWebPage(debug);
				case "Group-0-Item-3":
					return await PostPagesExample.CreatePageWithUrl(debug);
				case "Group-0-Item-4":
					return await PostPagesExample.CreatePageWithAttachedFile(debug);
				case "Group-0-Item-5":
					return await PostPagesExample.CreatePageWithNoteTags(debug);
				case "Group-0-Item-6":
					return await PostPagesExample.CreatePageWithAutoExtractBusinessCard(debug);
				case "Group-0-Item-7":
					return await PostPagesExample.CreatePageWithAutoExtractRecipe(debug);
				case "Group-0-Item-8":
					return await PostPagesExample.CreatePageWithAutoExtractProduct(debug);
				case "Group-0-Item-9":
					return await PostPagesExample.CreateSimplePageInAGivenSectionName(debug, requiredInputText);
				case "Group-0-Item-10":
					return await PostPagesExample.CreateSimplePageInAGivenSectionId(debug, requiredSelectedId);
				case "Group-1-Item-0":
					return await GetPagesExample.GetAllPages(debug);
				case "Group-1-Item-1":
					return await GetPagesExample.GetASpecificPageMetadata(debug, requiredSelectedId);
				case "Group-1-Item-2":
					return await GetPagesExample.GetAllPagesWithTitleContainsFilterQueryParams(debug, requiredInputText);
				case "Group-1-Item-3":
					return await GetPagesExample.GetAllPagesWithSkipAndTopQueryParams(debug, 20, 3);
				case "Group-1-Item-4":
					return await GetPagesExample.GetAllPagesWithOrderByAndSelectQueryParams(debug, "title asc", "id,title");
				case "Group-1-Item-5":
					return await GetPagesExample.SearchAllPages(debug, requiredInputText);
				case "Group-1-Item-6":
					return await GetPagesExample.GetASpecificPageContent(debug, requiredSelectedId);
                case "Group-1-Item-7":
                    return await GetPagesExample.GetAllPagesUnderASpecificSectionId(debug, requiredSelectedId);
				case "Group-2-Item-0":
					return await GetNotebooksExample.GetAllNotebooksExpand(debug);
				case "Group-2-Item-1":
					return await GetNotebooksExample.GetAllNotebooks(debug);
				case "Group-2-Item-2":
					return await GetNotebooksExample.GetASpecificNotebook(debug, requiredSelectedId);
				case "Group-2-Item-3":
					return await GetSectionsExample.GetAllSections(debug);
				case "Group-2-Item-4":
					return await GetSectionsExample.GetASpecificSection(debug, requiredSelectedId);
				case "Group-2-Item-5":
					return await GetSectionGroupsExample.GetAllSectionGroups(debug);
				case "Group-2-Item-6":
					return await GetNotebooksExample.GetAllNotebooksWithNameMatchingFilterQueryParam(debug, requiredInputText);
				case "Group-2-Item-7":
					return await GetNotebooksExample.GetAllNotebooksWithUserRoleAsNotOwnerFilterQueryParam(debug);
				case "Group-2-Item-8":
					return await GetNotebooksExample.GetAllNotebooksWithOrderByAndSelectQueryParams(debug, "name asc", "id,name");
				case "Group-2-Item-9":
					return await GetSectionsExample.GetSectionsUnderASpecificNotebook(debug, requiredSelectedId);
				case "Group-2-Item-10":
					return await GetSectionsExample.GetAllSectionsWithNameMatchingFilterQueryParam(debug, requiredInputText);
				case "Group-2-Item-11":
					return await PostNotebooksExample.CreateSimpleNotebook(debug, requiredInputText);
				case "Group-2-Item-12":
					return await PostSectionsExample.CreateSimpleSection(debug, requiredSelectedId, requiredInputText);
				case "Group-3-Item-0":
					return await PatchPagesExample.AppendToDefaultOutlineInPageContent(debug, requiredSelectedId);
			}
			return null;
		}

		public static async Task<object> ExecuteApiPrereq(string uniqueId)
		{
			switch (uniqueId)
			{
				case "Group-0-Item-10":
                case "Group-1-Item-7":
					return await GetNotebooksExample.GetAllNotebooksExpand(false);
				case "Group-1-Item-1":
					return await GetPagesExample.GetAllPages(false);
				case "Group-1-Item-6":
					return await GetPagesExample.GetAllPages(false);
				case "Group-2-Item-2":
					return await GetNotebooksExample.GetAllNotebooks(false);
				case "Group-2-Item-4":
					return await GetSectionsExample.GetAllSections(false);
				case "Group-2-Item-9":
					return await GetNotebooksExample.GetAllNotebooks(false);
				case "Group-2-Item-12":
					return await GetNotebooksExample.GetAllNotebooks(false);
				case "Group-3-Item-0":
					return await GetPagesExample.GetAllPages(false);

			}
			return null;
		}
	}
}