'
' DotNetNukeŽ - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.UI.Modules
Imports System.Collections.Generic
Imports DotNetNuke.Security.Permissions


Namespace DotNetNuke.Modules.Admin.Modules

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ModuleSettingsPage PortalModuleBase is used to edit the settings for a 
    ''' module.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	10/18/2004	documented
    ''' 	[cnurse]	10/19/2004	modified to support custm module specific settings
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ModuleSettingsPage
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private _Control As Control
        Private Shadows ModuleId As Integer = -1
        Private Shadows TabModuleId As Integer = -1
        Private _Module As ModuleInfo
		Private _TabsModulePaneInfo As IDictionary(Of Integer, String) = Nothing

#End Region

        Protected ReadOnly Property [Module]() As ModuleInfo
            Get
                If _Module Is Nothing Then
					_Module = ModuleCtrl.GetModule(ModuleId, TabId, False)
                End If
                Return _Module
            End Get
        End Property

        Protected ReadOnly Property SettingsControl() As ISettingsControl
            Get
                Return TryCast(_Control, ISettingsControl)
            End Get
        End Property

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindData loads the settings from the Database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/18/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindData()
            If Not [Module] Is Nothing Then
                ' configure grid
                Dim objDeskMod As New DesktopModuleController
                Dim desktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModule([Module].DesktopModuleID, PortalId)
                dgPermissions.ResourceFile = Common.Globals.ApplicationPath + "/DesktopModules/" + desktopModule.FolderName + "/" + Localization.LocalResourceDirectory + "/" + Localization.LocalSharedResourceFile


                chkInheritPermissions.Checked = [Module].InheritViewPermissions
                dgPermissions.InheritViewPermissionsFromTab = [Module].InheritViewPermissions

                txtFriendlyName.Text = [Module].DesktopModule.FriendlyName
                txtTitle.Text = [Module].ModuleTitle
                ctlIcon.Url = [Module].IconFile

                If cboTab.Items.FindByValue([Module].TabID.ToString) IsNot Nothing Then
                    cboTab.Items.FindByValue([Module].TabID.ToString).Selected = True
                End If
                If cboTab.Items.Count = 1 Then
                    rowTab.Visible = False
                Else
                    rowTab.Visible = True
                End If

                chkAllTabs.Checked = [Module].AllTabs
                cboVisibility.SelectedIndex = [Module].Visibility

				If Not IsPostBack Then
					BindInstalledOnPagesData()
				End If

				Dim objModuleDef As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinitionByID([Module].ModuleDefID)
				If objModuleDef.DefaultCacheTime = Null.NullInteger Then
					rowCache.Visible = False
				Else
					txtCacheTime.Text = [Module].CacheTime.ToString
				End If

				cboAlign.Items.FindByValue([Module].Alignment).Selected = True
				txtColor.Text = [Module].Color
				txtBorder.Text = [Module].Border

				txtHeader.Text = [Module].Header
				txtFooter.Text = [Module].Footer

				If Not Null.IsNull([Module].StartDate) Then
					txtStartDate.Text = [Module].StartDate.ToShortDateString
				End If
				If Not Null.IsNull([Module].EndDate) Then
					txtEndDate.Text = [Module].EndDate.ToShortDateString
				End If

				ctlModuleContainer.Width = "250px"
				ctlModuleContainer.SkinRoot = SkinController.RootContainer
				ctlModuleContainer.SkinSrc = [Module].ContainerSrc

				chkDisplayTitle.Checked = [Module].DisplayTitle
				chkDisplayPrint.Checked = [Module].DisplayPrint
				chkDisplaySyndicate.Checked = [Module].DisplaySyndicate
				chkWebSlice.Checked = [Module].IsWebSlice
				tblWebSlice.Visible = [Module].IsWebSlice
				txtWebSliceTitle.Text = [Module].WebSliceTitle
				If Not Null.IsNull([Module].WebSliceExpiryDate) Then
					txtWebSliceExpiry.Text = [Module].WebSliceExpiryDate.ToShortDateString
				End If
				If Not Null.IsNull([Module].WebSliceTTL) Then
					txtWebSliceTTL.Text = [Module].WebSliceTTL
				End If

				If [Module].ModuleID = PortalSettings.Current.DefaultModuleId AndAlso [Module].TabID = PortalSettings.Current.DefaultTabId Then
					chkDefault.Checked = True
				End If
			End If
        End Sub

		Private Sub BindInstalledOnPagesData()
			lblInstalledOn.HelpText = Localization.GetString("InstalledOn.Help", Me.LocalResourceFile)

			Dim tabsByModule As Dictionary(Of Integer, TabInfo) = TabCtrl.GetTabsByModuleID(ModuleId)
			tabsByModule.Remove(TabId) ' remove this tab
			If (tabsByModule.Count = 0) Then
				lstInstalledOnTabs.Visible = False
				lblInstalledOn.Text = Localization.GetString("MsgInstalledOnNone", Me.LocalResourceFile)
			Else
				lstInstalledOnTabs.Visible = True
				lblInstalledOn.Text = Localization.GetString("InstalledOn", Me.LocalResourceFile)
				lstInstalledOnTabs.DataSource = tabsByModule.Values
				lstInstalledOnTabs.DataBind()
			End If
		End Sub

#End Region

#Region "Properties"

		Private _ModuleCtrl As ModuleController = Nothing
		Protected ReadOnly Property ModuleCtrl() As ModuleController
			Get
				If (_ModuleCtrl Is Nothing) Then
					_ModuleCtrl = New ModuleController()
				End If
				Return _ModuleCtrl
			End Get
		End Property

		Private _TabCtrl As TabController = Nothing
		Protected ReadOnly Property TabCtrl() As TabController
			Get
				If (_TabCtrl Is Nothing) Then
					_TabCtrl = New TabController()
				End If
				Return _TabCtrl
			End Get
		End Property

#End Region

#Region "protected Methods"

		Protected Function GetInstalledOnLink(ByVal dataItem As Object) As String
			Dim returnValue As StringBuilder = New StringBuilder()
			If (TypeOf dataItem Is Entities.Tabs.TabInfo) Then
				Dim tab As Entities.Tabs.TabInfo = DirectCast(dataItem, Entities.Tabs.TabInfo)

				If (Not tab Is Nothing) Then
					Dim index As Integer = 0
					TabCtrl.PopulateBreadCrumbs(tab)

					For Each t As Entities.Tabs.TabInfo In tab.BreadCrumbs
						If (index > 0) Then
							returnValue.Append(" > ")
						End If

						If (tab.BreadCrumbs.Count - 1 = index) Then
							returnValue.AppendFormat("<a href=""{0}"">{1}</a>", t.FullUrl, t.LocalizedTabName)
						Else
							returnValue.AppendFormat("{0}", t.LocalizedTabName)
						End If

						index = index + 1
					Next
				End If
			End If
			Return returnValue.ToString()
		End Function

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            Dim objModules As New ModuleController
            Dim objModuleControlInfo As ModuleControlInfo
            Dim arrModuleControls As New ArrayList

            ' get ModuleId
            If Not (Request.QueryString("ModuleId") Is Nothing) Then
                ModuleId = Int32.Parse(Request.QueryString("ModuleId"))
            End If

            ' Verify that the current user has access to edit this module
            If Not ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "MANAGE", [Module]) Then
                Response.Redirect(AccessDeniedURL(), True)
            End If

            ' get module
            If Not [Module] Is Nothing Then
                TabModuleId = [Module].TabModuleID

                'get Settings Control
                objModuleControlInfo = ModuleControlController.GetModuleControlByControlKey("Settings", [Module].ModuleDefID)

                If objModuleControlInfo IsNot Nothing Then
                    _Control = ControlUtilities.LoadControl(Of Control)(Me.Page, objModuleControlInfo.ControlSrc)

                    Dim SettingsControl As ISettingsControl = TryCast(_Control, ISettingsControl)
                    If SettingsControl IsNot Nothing Then
                        'Set ID
                        _Control.ID = System.IO.Path.GetFileNameWithoutExtension(objModuleControlInfo.ControlSrc).Replace("."c, "-"c)

                        ' add module settings
                        SettingsControl.ModuleContext.Configuration = [Module]

                        dshSpecific.Text = Localization.GetString("ControlTitle_settings", SettingsControl.LocalResourceFile)
                        pnlSpecific.Controls.Add(_Control)

                        If Localization.GetString(Entities.Modules.Actions.ModuleActionType.HelpText, SettingsControl.LocalResourceFile) <> "" Then
                            rowspecifichelp.Visible = True
                            imgSpecificHelp.AlternateText = Localization.GetString(Entities.Modules.Actions.ModuleActionType.ModuleHelp, Localization.GlobalResourceFile)
                            lnkSpecificHelp.Text = Localization.GetString(Entities.Modules.Actions.ModuleActionType.ModuleHelp, Localization.GlobalResourceFile)
                            lnkSpecificHelp.NavigateUrl = NavigateURL(TabId, "Help", "ctlid=" & objModuleControlInfo.ModuleControlID.ToString, "moduleid=" & ModuleId)
                        Else
                            rowspecifichelp.Visible = False
                        End If
                    End If
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/18/2004	documented
        ''' 	[cnurse]	10/19/2004	modified to support custm module specific settings
        '''     [vmasanas]  11/28/2004  modified to support modules in admin tabs
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                'this needs to execute always to the client script code is registred in InvokePopupCal
                cmdStartCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtStartDate)
                cmdEndCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtEndDate)
                cmdWebSliceExpiry.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtWebSliceExpiry)

                If ModuleId <> -1 Then
                    ctlAudit.Entity = [Module]
                End If

                If Page.IsPostBack = False Then
                    ctlIcon.FileFilter = glbImageFileTypes

                    dgPermissions.TabId = PortalSettings.ActiveTab.TabID
                    dgPermissions.ModuleID = ModuleId

                    DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem"))

                    cboTab.DataSource = TabController.GetPortalTabs(PortalId, -1, False, Null.NullString, True, False, True, False, True)
                    cboTab.DataBind()

                    'if tab is a  host tab, then add current tab
                    If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                        cboTab.Items.Insert(0, New ListItem(PortalSettings.ActiveTab.LocalizedTabName, PortalSettings.ActiveTab.TabID.ToString))
                    End If

                    ' parent tab might not be loaded in cbotab if user does not have edit rights on it
                    If [Module] IsNot Nothing Then
                        If cboTab.Items.FindByValue([Module].TabID.ToString) Is Nothing Then
                            Dim objtabs As New TabController
                            Dim objTab As TabInfo = objtabs.GetTab([Module].TabID, PortalId, False)
                            cboTab.Items.Add(New ListItem(objTab.LocalizedTabName, objTab.TabID.ToString))
                        End If
                    End If

                    'only Portal Administrators can manage the visibility on all Tabs
                    trAllTabs.Visible = PortalSecurity.IsInRole("Administrators")

                    ' tab administrators can only manage their own tab
                    If Not TabPermissionController.CanAdminPage() Then
                        chkAllModules.Enabled = False
                        chkDefault.Enabled = False
                        cboTab.Enabled = False
                    End If

                    If ModuleId <> -1 Then
                        BindData()
                        cmdDelete.Visible = ModulePermissionController.CanDeleteModule([Module]) OrElse _
                                            TabPermissionController.CanAddContentToPage()
                    Else
                        cboVisibility.SelectedIndex = 0       ' maximized
                        chkAllTabs.Checked = False
                        cmdDelete.Visible = False
                    End If
                    cmdUpdate.Visible = ModulePermissionController.HasModulePermission([Module].ModulePermissions, "EDIT,MANAGE") OrElse _
                                            TabPermissionController.CanAddContentToPage()
					rowPerm.Visible = ModulePermissionController.CanAdminModule([Module]) OrElse _
											TabPermissionController.CanAddContentToPage()

                    'Set visibility of Specific Settings
                    If SettingsControl Is Nothing = False Then
                        'Get the module settings from the PortalSettings and pass the
                        'two settings hashtables to the sub control to process
                        SettingsControl.LoadSettings()
                        dshSpecific.Visible = True
                        tblSpecific.Visible = True
                    Else
                        dshSpecific.Visible = False
                        tblSpecific.Visible = False
                    End If

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub chkAllTabs_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkAllTabs.CheckedChanged
            If (Not [Module].AllTabs = chkAllTabs.Checked) Then
                trnewPages.Visible = chkAllTabs.Checked
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' chkInheritPermissions_CheckedChanged runs when the Inherit View Permissions
        '''	check box is changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/18/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub chkInheritPermissions_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkInheritPermissions.CheckedChanged
            If chkInheritPermissions.Checked Then
                dgPermissions.InheritViewPermissionsFromTab = True
            Else
                dgPermissions.InheritViewPermissionsFromTab = False
            End If
        End Sub

        Protected Sub chkWebSlice_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkWebSlice.CheckedChanged
            tblWebSlice.Visible = chkWebSlice.Checked
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the Cancel LinkButton is clicked.  It returns the user
        ''' to the referring page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/18/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL(), True)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdDelete_Click runs when the Delete LinkButton is clicked.
        ''' It deletes the current portal form the Database.  It can only run in Host
        ''' (SuperUser) mode
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/18/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
            Try
                Dim objModules As New ModuleController
                objModules.DeleteTabModule(TabId, ModuleId, True)
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the Update LinkButton is clicked.
        ''' It saves the current Site Settings
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/18/2004	documented
        ''' 	[cnurse]	10/19/2004	modified to support custm module specific settings
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal Sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try
                If Page.IsValid Then
                    Dim objModules As New ModuleController
                    Dim AllTabsChanged As Boolean = False

                    ' tab administrators can only manage their own tab
                    If Not TabPermissionController.CanAdminPage() Then
                        chkAllTabs.Enabled = False
                        chkDefault.Enabled = False
                        chkAllModules.Enabled = False
                        cboTab.Enabled = False
                    End If

                    ' update module
                    Dim objModule As ModuleInfo = objModules.GetModule(ModuleId, TabId, False)

                    objModule.ModuleID = ModuleId
                    objModule.ModuleTitle = txtTitle.Text
                    objModule.Alignment = cboAlign.SelectedItem.Value
                    objModule.Color = txtColor.Text
                    objModule.Border = txtBorder.Text
                    objModule.IconFile = ctlIcon.Url
                    If txtCacheTime.Text <> "" Then
                        objModule.CacheTime = Int32.Parse(txtCacheTime.Text)
                    Else
                        objModule.CacheTime = 0
                    End If
                    objModule.TabID = TabId
                    If Not objModule.AllTabs = chkAllTabs.Checked Then
                        AllTabsChanged = True
                    End If
                    objModule.AllTabs = chkAllTabs.Checked
                    Select Case Int32.Parse(cboVisibility.SelectedItem.Value)
                        Case 0 : objModule.Visibility = VisibilityState.Maximized
                        Case 1 : objModule.Visibility = VisibilityState.Minimized
                        Case 2 : objModule.Visibility = VisibilityState.None
                    End Select
                    objModule.IsDeleted = False
                    objModule.Header = txtHeader.Text
                    objModule.Footer = txtFooter.Text
                    If Not String.IsNullOrEmpty(txtStartDate.Text) Then
                        objModule.StartDate = Convert.ToDateTime(txtStartDate.Text)
                    Else
                        objModule.StartDate = Null.NullDate
                    End If
                    If Not String.IsNullOrEmpty(txtEndDate.Text) Then
                        objModule.EndDate = Convert.ToDateTime(txtEndDate.Text)
                    Else
                        objModule.EndDate = Null.NullDate
                    End If
                    objModule.ContainerSrc = ctlModuleContainer.SkinSrc

                    objModule.ModulePermissions.Clear()
                    objModule.ModulePermissions.AddRange(dgPermissions.Permissions)

                    objModule.InheritViewPermissions = chkInheritPermissions.Checked
                    objModule.DisplayTitle = chkDisplayTitle.Checked
                    objModule.DisplayPrint = chkDisplayPrint.Checked
                    objModule.DisplaySyndicate = chkDisplaySyndicate.Checked
                    objModule.IsWebSlice = chkWebSlice.Checked
                    objModule.WebSliceTitle = txtWebSliceTitle.Text
                    If Not String.IsNullOrEmpty(txtWebSliceExpiry.Text) Then
                        objModule.WebSliceExpiryDate = Convert.ToDateTime(txtWebSliceExpiry.Text)
                    Else
                        objModule.WebSliceExpiryDate = Null.NullDate
                    End If
                    If Not String.IsNullOrEmpty(txtWebSliceTTL.Text) Then
                        objModule.WebSliceTTL = Convert.ToInt32(txtWebSliceTTL.Text)
                    End If
                    objModule.IsDefaultModule = chkDefault.Checked
                    objModule.AllModules = chkAllModules.Checked
                    objModules.UpdateModule(objModule)

                    'Update Custom Settings
                    If SettingsControl IsNot Nothing Then
                        Try
                            SettingsControl.UpdateSettings()
                        Catch ex As System.Threading.ThreadAbortException
                            System.Threading.Thread.ResetAbort() ' necessary
                        Catch ex As Exception
                            Exceptions.LogException(ex)
                        End Try
                    End If

                    'These Module Copy/Move statements must be 
                    'at the end of the Update as the Controller code assumes all the 
                    'Updates to the Module have been carried out.

                    'Check if the Module is to be Moved to a new Tab
                    If Not chkAllTabs.Checked Then
                        Dim newTabId As Integer = Int32.Parse(cboTab.SelectedItem.Value)
                        If TabId <> newTabId Then
                            'First check if there already is an instance of the module on the target page
                            Dim tmpModule As ModuleInfo = objModules.GetModule(ModuleId, newTabId)
                            If tmpModule Is Nothing Then
                                'Move module
                                objModules.MoveModule(ModuleId, TabId, newTabId, "")
                            Else
                                'Warn user
                                Skin.AddModuleMessage(Me, Localization.GetString("ModuleExists", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                                Exit Sub
                            End If
                        End If
                    End If

                    ''Check if Module is to be Added/Removed from all Tabs
                    If AllTabsChanged Then
                        Dim listTabs As List(Of Entities.Tabs.TabInfo) = TabController.GetPortalTabs(PortalSettings.PortalId, Null.NullInteger, False, True)
                        If chkAllTabs.Checked Then
                            If Not chkNewTabs.Checked Then
                                objModules.CopyModule(ModuleId, TabId, listTabs, True)
                            End If
                        Else
                            objModules.DeleteAllModules(ModuleId, TabId, listTabs)
                        End If
                    End If

                    ' Navigate back to admin page
                    Response.Redirect(NavigateURL(), True)

                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

		Protected Sub lstInstalledOnTabs_PageIndexChanged(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles lstInstalledOnTabs.PageIndexChanged
			Try
				lstInstalledOnTabs.CurrentPageIndex = e.NewPageIndex
				BindInstalledOnPagesData()
			Catch ex As Exception
				ProcessModuleLoadException(Me, ex)
			End Try
		End Sub
#End Region

    End Class

End Namespace
