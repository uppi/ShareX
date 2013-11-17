﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using HelpersLib;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ImageEffectsLib
{
    public partial class WatermarkForm : Form
    {
        private WatermarkConfig config;
        private bool IsGuiReady;
        private ContextMenuStrip codesMenu;

        public WatermarkForm(WatermarkConfig watermarkConfig)
        {
            InitializeComponent();
            config = watermarkConfig;
            codesMenu = NameParser.CreateCodesMenu(txtWatermarkText, ReplacementVariables.t);
        }

        private void WatermarkUI_Load(object sender, EventArgs e)
        {
            if (cboWatermarkType.Items.Count == 0)
            {
                cboWatermarkType.Items.AddRange(Enum.GetNames(typeof(WatermarkType)));
            }

            cboWatermarkType.SelectedIndex = (int)config.Type;

            if (chkWatermarkPosition.Items.Count == 0)
            {
                chkWatermarkPosition.Items.AddRange(Enum.GetNames(typeof(ContentAlignment)));
            }

            chkWatermarkPosition.SelectedIndex = config.Text.Placement.GetIndex();
            nudWatermarkOffset.Value = config.Text.Offset.X;
            cbWatermarkAutoHide.Checked = config.Text.AutoHide;

            txtWatermarkText.Text = config.Text.Text;
            lblWatermarkFont.Text = new FontConverter().ConvertToInvariantString(config.Text.TextFont);
            btnTextColor.Color = config.Text.TextColor;

            cbWatermarkDrawBackground.Checked = config.Text.DrawBackground;
            btnBorderColor.Color = config.Text.BorderColor;
            nudWatermarkCornerRadius.Value = config.Text.CornerRadius;
            btnBackgroundColor.Color = config.Text.BackgroundColor;
            cbWatermarkUseGradient.Checked = config.Text.UseGradient;
            btnBackgroundColor2.Color = config.Text.BackgroundColor2;

            if (cbWatermarkGradientType.Items.Count == 0)
            {
                cbWatermarkGradientType.Items.AddRange(Enum.GetNames(typeof(LinearGradientMode)));
            }

            cbWatermarkGradientType.SelectedIndex = (int)config.Text.GradientType;

            txtWatermarkImageLocation.Text = config.Image.ImageLocation;

            IsGuiReady = true;
            UpdatePreview();
        }

        private void WatermarkUI_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void UpdatePreview()
        {
            if (IsGuiReady)
            {
                using (Bitmap bmp = new Bitmap(pbPreview.ClientSize.Width, pbPreview.ClientSize.Height))
                {
                    config.Apply(bmp);
                    pbPreview.LoadImage(bmp);
                }
            }
        }

        private void cboWatermarkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            config.Type = (WatermarkType)cboWatermarkType.SelectedIndex;
            UpdatePreview();
        }

        private void cbWatermarkPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            config.Text.Placement = Helpers.GetEnumFromIndex<ContentAlignment>(chkWatermarkPosition.SelectedIndex);
            UpdatePreview();
        }

        private void nudWatermarkOffset_ValueChanged(object sender, EventArgs e)
        {
            config.Text.Offset = new Point((int)nudWatermarkOffset.Value, (int)nudWatermarkOffset.Value);
            UpdatePreview();
        }

        private void cbWatermarkAutoHide_CheckedChanged(object sender, EventArgs e)
        {
            config.Text.AutoHide = cbWatermarkAutoHide.Checked;
            UpdatePreview();
        }

        private void txtWatermarkText_TextChanged(object sender, EventArgs e)
        {
            config.Text.Text = txtWatermarkText.Text;
            UpdatePreview();
        }

        private void btnWatermarkFont_Click(object sender, EventArgs e)
        {
            try
            {
                using (FontDialog fontDialog = new FontDialog())
                {
                    fontDialog.ShowColor = true;

                    try
                    {
                        fontDialog.Font = config.Text.TextFont;
                        fontDialog.Color = config.Text.TextColor;
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteException(ex, "Error while initializing font.");
                    }

                    if (fontDialog.ShowDialog() == DialogResult.OK)
                    {
                        config.Text.TextFont = fontDialog.Font;
                        config.Text.TextColor = fontDialog.Color;

                        btnTextColor.Color = config.Text.TextColor;
                        lblWatermarkFont.Text = new FontConverter().ConvertToInvariantString(config.Text.TextFont);
                        UpdatePreview();
                    }
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex, "Error while setting watermark font.");
            }
        }

        private void btnTextColor_ColorChanged(Color color)
        {
            config.Text.TextColor = color;
            UpdatePreview();
        }

        private void cbWatermarkDrawBackground_CheckedChanged(object sender, EventArgs e)
        {
            config.Text.DrawBackground = cbWatermarkDrawBackground.Checked;
            UpdatePreview();
        }

        private void btnBorderColor_ColorChanged(Color color)
        {
            config.Text.BorderColor = color;
            UpdatePreview();
        }

        private void nudWatermarkCornerRadius_ValueChanged(object sender, EventArgs e)
        {
            config.Text.CornerRadius = (int)nudWatermarkCornerRadius.Value;
            UpdatePreview();
        }

        private void btnBackgroundColor_ColorChanged(Color color)
        {
            config.Text.BackgroundColor = color;
            UpdatePreview();
        }

        private void cbWatermarkBackColor2_CheckedChanged(object sender, EventArgs e)
        {
            config.Text.UseGradient = cbWatermarkUseGradient.Checked;
            btnBackgroundColor2.Enabled = config.Text.UseGradient;
            cbWatermarkGradientType.Enabled = config.Text.UseGradient;
            UpdatePreview();
        }

        private void cbWatermarkGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            config.Text.GradientType = (LinearGradientMode)cbWatermarkGradientType.SelectedIndex;
            UpdatePreview();
        }

        private void btnBackgroundColor2_ColorChanged(Color color)
        {
            config.Text.BackgroundColor2 = color;
            UpdatePreview();
        }

        private void txtWatermarkImageLocation_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtWatermarkImageLocation.Text))
            {
                config.Image.ImageLocation = txtWatermarkImageLocation.Text;
                UpdatePreview();
            }
        }

        private void btwWatermarkBrowseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtWatermarkImageLocation.Text = ofd.FileName;
                }
            }
        }
    }
}