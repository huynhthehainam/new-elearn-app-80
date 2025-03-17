/// <summary>Kendo UI ComboBoxGrid Plugin.</summary>
/// <description>Demonstrate a Kendo UI ComboBox Plugin with a Kendo Grid as the dropdown list.</description>
/// <version>1.0</version>
/// <author>John DeVight</author>
/// <license>
/// Licensed under the MIT License (MIT)
/// You may obtain a copy of the License at
/// http://opensource.org/licenses/mit-license.html
/// </license>
(function ($, kendo) {
    var ExtComboBoxGrid = kendo.ui.ComboBox.extend({
        options: {
            name: "ExtComboBoxGrid",
            grid: {
                selectable: "row"
            }
        },

        _grid: null,
        
        init: function (element, options) {
            /// <summary>
            /// Initialize the widget.
            /// </summary>
        
            var that = this;
            
            // Call the base class init.
            kendo.ui.ComboBox.fn.init.call(this, element, options);

            // Replace the dropdownlist with a grid.
            this.list.html("<div class='k-ext-grid'></div>");
            this._grid = this.list.find(".k-ext-grid").kendoGrid(
                $.extend({}, this.options.grid, { dataSource: options.dataSource })
            ).data("kendoGrid");
            
            // When the combobox dropdown is displayed for the first time, resize the grid.
            this.one("open", function () {
                setTimeout(function () {
                var height = typeof that.options.grid.height === "undefined"
                  ? 400
                  : that.options.grid.height;

                that._grid.wrapper.height(height);
                that._grid.resize();
              }, 100);
            });
            
            this._grid.bind("change", function () {
                /// <summary>
                /// When an item is selected in the grid, select the item in the combobox.
                /// </summary>

                var selectedRows = this.select();
                var dataItem = this.dataItem(selectedRows[0]);

                that.select(function (item) {
                    return item[that.options.dataValueField] === dataItem[that.options.dataValueField];
                });

                that.trigger("change");
                that.close();
            });

            this.popup.element.on("click", "form.k-filter-menu input.k-textbox", function () {
                /// <summary>
                /// Let the user click into the filter menu textbox.
                /// </summary>

                $(this).focus();
            });
        },
        
        setDataSource: function (dataSource) {
          /// <summary>
          /// When setting the datasource, set the grid datasource.
          /// </summary>

            kendo.ui.ComboBox.fn.setDataSource.call(this, dataSource);

            this._grid.setDataSource(dataSource);
        }
    });
    kendo.ui.plugin(ExtComboBoxGrid);
})(window.kendo.jQuery, window.kendo);