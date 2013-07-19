﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectItem.cs" company="Caitlyn development team">
//   Copyright (c) 2008 - 2013 Caitlyn development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Caitlyn.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.Serialization;

    using Catel;
    using Catel.Data;

    using EnvDTE;

    /// <summary>
    /// ProjectItem Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    [Serializable]
    public class ProjectItem : ModelBase, IProjectItem
    {
        #region Constructor & destructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItem"/> class.
        /// </summary>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="project"/> is <c>null</c>.
        /// </exception>
        public ProjectItem(Project project)
        {
            Argument.IsNotNull("project", project);

            Initialize(string.Empty, ProjectItemType.Folder);

            foreach (EnvDTE.ProjectItem subProjectItem in project.ProjectItems)
            {
                ProjectItems.Add(new ProjectItem(subProjectItem, this));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItem"/> class.
        /// </summary>
        /// <param name="projectItem">
        /// The project item.
        /// </param>
        /// <param name="parent">
        /// The parent. Can be <c>null</c> when this item represents the root.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="projectItem"/> is <c>null</c>.
        /// </exception>
        public ProjectItem(EnvDTE.ProjectItem projectItem, IProjectItem parent = null)
        {
            Argument.IsNotNull("projectItem", projectItem);

            Initialize(projectItem.GetObjectName(), projectItem.IsFolder() ? ProjectItemType.Folder : ProjectItemType.File, parent);

            foreach (EnvDTE.ProjectItem subProjectItem in projectItem.ProjectItems)
            {
                ProjectItems.Add(new ProjectItem(subProjectItem, this));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItem"/> class. 
        /// Initializes a new object from scratch.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="projectItemType">
        /// Type of the project item.
        /// </param>
        /// <param name="parent">
        /// The parent. Can be <c>null</c> when this item represents the root.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="name"/> is <c>null</c> or whitespace.
        /// </exception>
        public ProjectItem(string name, ProjectItemType projectItemType, IProjectItem parent = null)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            Initialize(name, projectItemType, parent);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItem"/> class. 
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info">
        /// <see cref="SerializationInfo"/> that contains the information.
        /// </param>
        /// <param name="context">
        /// <see cref="StreamingContext"/>.
        /// </param>
        protected ProjectItem(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the parent.
        /// </summary>
        public IProjectItem Parent { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the project item type.
        /// </summary>
        public ProjectItemType Type
        {
            get { return GetValue<ProjectItemType>(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        /// <summary>
        /// Register the Type property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TypeProperty = RegisterProperty("Type", typeof(ProjectItemType), ProjectItemType.File);

        /// <summary>
        /// Gets the project items that are a contained by this project item.
        /// </summary>
        public ObservableCollection<IProjectItem> ProjectItems
        {
            get { return GetValue<ObservableCollection<IProjectItem>>(ProjectItemsProperty); }
            private set { SetValue(ProjectItemsProperty, value); }
        }

        /// <summary>
        /// Register the ProjectItems property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ProjectItemsProperty = RegisterProperty("ProjectItems", typeof(ObservableCollection<IProjectItem>), () => new ObservableCollection<IProjectItem>());

        /// <summary>
        /// Gets the full name, which is calculated by walking up the tree via the <see cref="Parent"/> property.
        /// </summary>
        public string FullName
        {
            get
            {
                if (Parent == null)
                {
                    return Name;
                }

                return Path.Combine(Parent.FullName, Name);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the current project item.
        /// </summary>
        /// <param name="name">
        /// The name. Can only be whitespace when the <paramref name="parent"/> is <c>null</c>.
        /// </param>
        /// <param name="projectItemType">
        /// Type of the project item.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="name"/> is <c>null</c> or whitespace.
        /// </exception>
        private void Initialize(string name, ProjectItemType projectItemType, IProjectItem parent = null)
        {
            if (parent == null)
            {
                Argument.IsNotNull("name", name);
            }
            else
            {
                Argument.IsNotNullOrWhitespace("name", name);
            }

            Name = name;
            Type = projectItemType;
            Parent = parent;
        }
        #endregion
    }
}