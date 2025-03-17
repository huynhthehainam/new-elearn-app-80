var SQLMinDateTimeTotalMilliSeconds = -6847804800000;
var HttpMethod = { POST: "POST", GET: "GET", PUT: "PUT", DELETE: "DELETE" };
var DateFormat = {
    FormatAm: "DD MMM YYYY hh:mm A",
    FormatYmd: "YYYY/MM/DD",
    FormatWithD: "dddd, D MMM YYYY",
    FormatPicker: "MM/DD/YYYY hh:mm A",
    UTCShortMonth: "DD MMM YYYY HH:mm",
    DisplayDateTime: "DD MMM YYYY HH:mm",
    FormatDMY: "DD/MM/YYYY",
    HourAmPm: "hh:mm A"
};
var KendoDateFormat = { DateTimeDisplay: "dd MMM yyyy hh:mm tt" };

var DateType = { JSON: "json", XML: "xml" };
var Weeks = new Array(
    "Sunday",
    "Monday",
    "Tuesday",
    "Wednesday",
    "Thursday",
    "Friday",
    "Saturday"
);
var HttpStatusCode = { Unauthorized: 401, OK: 200 };
var ResponseStatusCode = {
    Fail: -1,
    DoNotAllow: 0,
    Success: 1,
    DoesNotExists: -2,
    NotAuthorized: -3,
    AlreadyExists: -4
};
var NavigateTag = {
    Rpt_Index: "index",
    Rpt_MassModeration: "massmoderation",
    Rpt_GradeDistribution: "gradedistribution",
    Rpt_GradeDistributionYourSection: "gradedistributionYourSection",
    Rpt_IGrade: "igrade",
    Rpt_MassModerationHistory: "mmhistory"
};

function request(url, param) {
    return new Promise((resolve, reject) => {
        const xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function Success() {
            if (this.readyState === xhr.DONE) {
                resolve(JSON.parse(xhr.responseText));
            }
        };
        xhr.open("POST", url, true);
        xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        xhr.send(param);
        xhr.onerror = function (error) {
            reject("Error");
        };
    });
}
function GetData(url, method, param) {
    return $.ajax({
        type: method,
        url: url,
        data: JSON.stringify(param),
        headers: {
            "Content-Type": "application/json"
        }
    });
}

function GetPage(url) {
    return fetch(url, {
        mode: "no-cors",
        method: HttpMethod.GET
    })
        .then(function (response) {
            return response.text();
        })
        .catch(function (err) {
            console.log(err);
        });
}
async function GetPageAsync(url) {
    const response = await fetch(url, {
        mode: "no-cors",
        method: HttpMethod.GET
    });
    const data = await response.text();
    return data;
}
function Execute(url, method, param) {
    return $.ajax({
        type: method,
        url: url,
        data: JSON.stringify(param),
        headers: { "Content-Type": "application/json" }
    });
}

function PostData(url, param) {
    return fetch(url, {
        body: param,
        method: HttpMethod.POST,
        headers: { "Content-type": "application/json" }
    })
        .then(response => response.json())
        .then(success => console.log(success))
        .catch(error => console.log(error));
}
async function PostDataAsync(url, param) {
    const response = await fetch(url, {
        method: HttpMethod.POST,
        body: param
    });
    const data = await response.json();
    return data;
}
function ExecuteWithOutHeader(url, method, param) {
    return fetch(url, {
        method: method,
        body: JSON.stringify(param)
    })
        .then(function (response) {
            return response.json();
        })
        .catch(function (err) {
            console.log(err);
        });
}
function GetLocalTime(millisecond, tz, format) {
    if (millisecond === null || millisecond === undefined || millisecond === 0) return undefined;
    if (format === undefined) {
        var dt = moment(millisecond).tz(tz);
        return dt.format();
    } else
        return moment(millisecond)
            .tz(tz)
            .format(format);
}
function addAntiForgeryToken(data) {
    if (!data) {
        data = {};
    }
    var tokenInput = $("input[name=__RequestVerificationToken]");
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
}

function LoadPartialView(url) {
    return fetch(url).then(function (response) {
        return response.text();
    });
}
function addExtensionClass(extension) {
    switch (extension) {
        case ".jpg":
        case ".img":
        case ".png":
        case ".gif":
            return "image";
        case ".doc":
        case ".docx":
            return "file-doc";
        case ".xls":
        case ".xlsx":
            return "file-xls";
        case ".pdf":
            return "file-pdf";
        case ".zip":
        case ".rar":
            return "file-zip";
        default:
            return "file-default";
    }
}
var getDates = function (startDate, endDate) {
    var dates = [],
        currentDate = startDate,
        addDays = function (days) {
            var date = new Date(
                this.getFullYear(),
                this.getMonth(),
                this.getDate(),
                0,
                0,
                0,
                0
            ); //remove time portion
            date.setDate(date.getDate() + days); // increment by days
            return date;
        };
    while (currentDate <= endDate) {
        dates.push(currentDate);
        currentDate = addDays.call(currentDate, 1);
    }
    return dates;
};
function ShowLoadingMask(element) {
    $(element).LoadingOverlay("show");
}
function HideLoadingMask(element) {
    $(element).LoadingOverlay("hide");
}
function NavigateByTagName(tagName) {
    $(`a[tag="#${tagName}"]`).click();
}
function GoBack() {
    window.history.back();
}
function RedirectToEntryPage(url) {
    //$("#main").html('');
    $("#main").load(url);
}
function RequestApi(url, method, param) {
    return fetch(url, {
        method: method,
        body: JSON.stringify(param),
        headers: { "Content-Type": "application/json" }
    })
        .then(function (response) {
            return response.json();
        })
        .catch(function (err) {
            console.log(err);
        });
}
async function RequestApiAsync(url, method, param) {
    let response = await fetch(url, {
        method: method,
        body: JSON.stringify(param),
        headers: { "Content-Type": "application/json" }
    });
    const data = await response.json();
    return data;
}
async function RequestPageAsync(url, method, param) {
    let response = await fetch(url, {
        method: method,
        body: JSON.stringify(param),
        headers: { "Content-Type": "application/json" }
    });
    const data = await response.text();
    return data;
}
function MemorySizeOf(obj) {
    function formatByteSize(bytes) {
        if (bytes < 1024) return bytes + " bytes";
        else if (bytes < 1048576) return (bytes / 1024).toFixed(3) + " KiB";
        else if (bytes < 1073741824) return (bytes / 1048576).toFixed(3) + " MiB";
        else return (bytes / 1073741824).toFixed(3) + " GiB";
    }
    return formatByteSize(sizeOf(obj));
}
function sizeOf(obj) {
    var bytes = 0;
    if (obj !== null && obj !== undefined) {
        switch (typeof obj) {
            case "number":
                bytes += 8;
                break;
            case "string":
                bytes += obj.length * 2;
                break;
            case "boolean":
                bytes += 4;
                break;
            case "object":
                var objClass = Object.prototype.toString.call(obj).slice(8, -1);
                if (objClass === "Object" || objClass === "Array") {
                    for (var key in obj) {
                        if (!obj.hasOwnProperty(key)) continue;
                        sizeOf(obj[key]);
                    }
                } else bytes += obj.toString().length * 2;
                break;
        }
    }
    return bytes;
}
function QuoteAttr(s, preserveCr) {
    preserveCr = preserveCr ? "&#13;" : "\n";
    return (
        ("" + s) /* Forces the conversion to string. */
            .replace(/&/g, "&amp;") /* This MUST be the 1st replacement. */
            .replace(/'/g, "&apos;") /* The 4 other predefined entities, required. */
            .replace(/"/g, "&quot;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            /*
              You may add other replacements here for HTML only 
              (but it's not necessary).
              Or for XML, only if the named entities are defined in its DTD.
              */
            .replace(/\r\n/g, preserveCr) /* Must be before the next replacement. */
            .replace(/[\r\n]/g, preserveCr)
    );
}
function UpperCaseFirstLetter(inputText) {
    inputText = inputText
        .toLowerCase()
        .split(" ")
        .map(s => s.charAt(0).toUpperCase() + s.substring(1))
        .join(" ");
    return inputText;
}
function fixTable(container) {
    // Store references to table elements
    var thead = container.querySelector("thead");
    var tbody = container.querySelector("tbody");

    // Style container
    container.style.overflow = "auto";
    container.style.position = "relative";

    // Add inline styles to fix the header row and leftmost column
    function relayout() {
        var ths = [].slice.call(thead.querySelectorAll("th"));
        var tbodyTrs = [].slice.call(tbody.querySelectorAll("tr"));

        /**
         * Remove inline styles so we resort to the default table layout algorithm
         * For thead, th, and td elements, don't remove the 'transform' styles applied
         * by the scroll event listener
         */
        tbody.setAttribute("style", "");
        thead.style.width = "";
        thead.style.position = "";
        thead.style.top = "";
        thead.style.left = "";
        thead.style.zIndex = "";
        ths.forEach(function (th) {
            th.style.display = "";
            th.style.width = "";
            th.style.position = "";
            th.style.top = "";
            th.style.left = "";
        });
        tbodyTrs.forEach(function (tr) {
            tr.setAttribute("style", "");
        });
        [].slice.call(tbody.querySelectorAll("td")).forEach(function (td) {
            td.style.width = "";
            td.style.position = "";
            td.style.left = "";
        });

        /**
         * Store width and height of each th
         * getBoundingClientRect()'s dimensions include paddings and borders
         */
        var thStyles = ths.map(function (th) {
            var rect = th.getBoundingClientRect();
            var style = document.defaultView.getComputedStyle(th, "");
            return {
                boundingWidth: rect.width,
                boundingHeight: rect.height,
                width: parseInt(style.width, 10),
                paddingLeft: parseInt(style.paddingLeft, 10)
            };
        });

        // Set widths of thead and tbody
        var totalWidth = thStyles.reduce(function (sum, cur) {
            return sum + cur.boundingWidth;
        }, 0);
        tbody.style.display = "block";
        tbody.style.width = totalWidth + "px";
        thead.style.width = totalWidth - thStyles[0].boundingWidth + "px";

        // Position thead
        thead.style.position = "absolute";
        thead.style.top = "0";
        thead.style.left = thStyles[0].boundingWidth + "px";
        thead.style.zIndex = 10;

        // Set widths of the th elements in thead. For the fixed th, set its position
        ths.forEach(function (th, i) {
            th.style.width = thStyles[i].width + "px";
            if (i === 0) {
                th.style.position = "absolute";
                th.style.top = "0";
                th.style.left = -thStyles[0].boundingWidth + "px";
            }
        });

        // Set margin-top for tbody - the fixed header is displayed in this margin
        tbody.style.marginTop = thStyles[0].boundingHeight + "px";

        // Set widths of the td elements in tbody. For the fixed td, set its position
        tbodyTrs.forEach(function (tr, i) {
            tr.style.display = "block";
            tr.style.paddingLeft = thStyles[0].boundingWidth + "px";
            [].slice.call(tr.querySelectorAll("td")).forEach(function (td, j) {
                td.style.width = thStyles[j].width + "px";
                if (j === 0) {
                    td.style.position = "absolute";
                    td.style.left = "0";
                }
            });
        });
    }

    // Initialize table styles
    relayout();

    // Update table cell dimensions on resize
    window.addEventListener("resize", resizeThrottler, false);
    var resizeTimeout;
    function resizeThrottler() {
        if (!resizeTimeout) {
            resizeTimeout = setTimeout(function () {
                resizeTimeout = null;
                relayout();
            }, 500);
        }
    }

    // Fix thead and first column on scroll
    container.addEventListener("scroll", function () {
        thead.style.transform = "translate3d(0," + this.scrollTop + "px,0)";
        var hTransform = "translate3d(" + this.scrollLeft + "px,0,0)";
        thead.querySelector("th").style.transform = hTransform;
        [].slice
            .call(tbody.querySelectorAll("tr > td:first-child"))
            .forEach(function (td, i) {
                td.style.transform = hTransform;
            });
    });

    /**
     * Return an object that exposes the relayout function so that we can
     * update the table when the number of columns or the content inside columns changes
     */
    return {
        relayout: relayout
    };
}
$.fn.isBound = function (type, fn) {
    var data = this.data("events")[type];

    if (data === undefined || data.length === 0) {
        return false;
    }

    return -1 !== $.inArray(fn, data);
};
var SortType = {
    Asc: 1,
    Desc: 2
};
const capitalize = s => {
    if (typeof s !== "string") return "";
    return s.charAt(0).toUpperCase() + s.slice(1);
};
function wordCount(value) {
    return $.trim(value.replace(/<.*?>/g, " "))
        .replace(/['";:,.?\-!]+/g, "")
        .match(/\S+/g).length;
}
function ucs2ToBinaryString(str) {
    if (str === null || str === undefined || str.length === 0) return "";
    str = str.replace(/(\r\n|\n|\r)/g, "");
    var escStr = encodeURIComponent(str);
    var binStr = escStr.replace(/%([0-9A-F]{2})/gi, function (match, hex) {
        var i = parseInt(hex, 16);
        return String.fromCharCode(i);
    });
    return binStr;
}
function encode_utf8(s) {
    return unescape(encodeURIComponent(s));
}

function substr_utf8_bytes(str, startInBytes, lengthInBytes) {
    /* this function scans a multibyte string and returns a substring. 
       * arguments are start position and length, both defined in bytes.
       * 
       * this is tricky, because javascript only allows character level 
       * and not byte level access on strings. Also, all strings are stored
       * in utf-16 internally - so we need to convert characters to utf-8
       * to detect their length in utf-8 encoding.
       *
       * the startInBytes and lengthInBytes parameters are based on byte 
       * positions in a utf-8 encoded string.
       * in utf-8, for example: 
       *       "a" is 1 byte, 
               "ü" is 2 byte, 
          and  "你" is 3 byte.
       *
       * NOTE:
       * according to ECMAScript 262 all strings are stored as a sequence
       * of 16-bit characters. so we need a encode_utf8() function to safely
       * detect the length our character would have in a utf8 representation.
       * 
       * http://www.ecma-international.org/publications/files/ecma-st/ECMA-262.pdf
       * see "4.3.16 String Value":
       * > Although each value usually represents a single 16-bit unit of 
       * > UTF-16 text, the language does not place any restrictions or 
       * > requirements on the values except that they be 16-bit unsigned 
       * > integers.
       */

    var resultStr = "";
    var startInChars = 0;

    // scan string forward to find index of first character
    // (convert start position in byte to start position in characters)

    for (bytePos = 0; bytePos < startInBytes; startInChars++) {
        // get numeric code of character (is >128 for multibyte character)
        // and increase "bytePos" for each byte of the character sequence

        ch = str.charCodeAt(startInChars);
        bytePos += ch < 128 ? 1 : encode_utf8(str[startInChars]).length;
    }

    // now that we have the position of the starting character,
    // we can built the resulting substring

    // as we don't know the end position in chars yet, we start with a mix of
    // chars and bytes. we decrease "end" by the byte count of each selected
    // character to end up in the right position
    end = startInChars + lengthInBytes - 1;

    for (n = startInChars; startInChars <= end; n++) {
        // get numeric code of character (is >128 for multibyte character)
        // and decrease "end" for each byte of the character sequence
        ch = str.charCodeAt(n);
        end -= ch < 128 ? 1 : encode_utf8(str[n]).length;

        resultStr += str[n];
    }

    return resultStr;
}
function fixedCharCodeAt(str, idx) {
    idx = idx || 0;
    var code = str.charCodeAt(idx);
    var hi, low;
    if (0xD800 <= code && code <= 0xDBFF) { // High surrogate (could change last hex to 0xDB7F to treat high private surrogates as single characters)
        hi = code;
        low = str.charCodeAt(idx + 1);
        if (isNaN(low)) {
            throw 'something went wrong!';
        }
        return ((hi - 0xD800) * 0x400) + (low - 0xDC00) + 0x10000;
    }
    if (0xDC00 <= code && code <= 0xDFFF) { // Low surrogate
        // We return false to allow loops to skip this iteration since should have already handled high surrogate above in the previous iteration
        return false;
        /*hi = str.charCodeAt(idx-1);
         low = code;
         return ((hi - 0xD800) * 0x400) + (low - 0xDC00) + 0x10000;*/
    }
    return code;
}
function countUtf8(str) {
    var result = 0;
    for (var n = 0; n < str.length; n++) {
        var charCode = fixedCharCodeAt(str, n);
        if (typeof charCode === "number") {
            if (charCode < 128) {
                result = result + 1;
            } else if (charCode < 2048) {
                result = result + 2;
            } else if (charCode < 65536) {
                result = result + 3;
            } else if (charCode < 2097152) {
                result = result + 4;
            } else if (charCode < 67108864) {
                result = result + 5;
            } else {
                result = result + 6;
            }
        }
    }
    return result;
}
function dt_print(e, dt, button, config) {
    var data = dt.buttons.exportData(
        $.extend({ decodeEntities: false }, config.exportOptions) // XSS protection
    );
    var columnClasses = dt
        .columns(config.exportOptions.columns)
        .flatten()
        .map(function (idx) {
            return dt.settings()[0].aoColumns[dt.column(idx).index()].sClass;
        })
        .toArray();

    var addRow = function (d, tag) {
        var str = '<tr>';
        for (let i = 0, ien = d.length; i < ien; i++) {
            // null and undefined aren't useful in the print output
            var dataOut = d[i] === null || d[i] === undefined ?
                '' :
                d[i];
            var classAttr = columnClasses[i] ?
                'class="' + columnClasses[i] + '"' :
                '';

            str += '<' + tag + ' ' + classAttr + '>' + dataOut + '</' + tag + '>';
        }
        return str + '</tr>';
    };

    // Construct a table for printing
    var html = '<table class="' + dt.table().node().className + '">';

    if (config.header) {
        html += '<thead>' + addRow(data.header, 'th') + '</thead>';
    }

    html += '<tbody>';
    for (var i = 0, ien = data.body.length; i < ien; i++) {
        html += addRow(data.body[i], 'td');
    }
    html += '</tbody>';

    if (config.footer && data.footer) {
        html += '<tfoot>' + addRow(data.footer, 'th') + '</tfoot>';
    }
    html += '</table>';

    return html;
}
var buildFormData = (formData, data, parentKey) => {
    if (data && typeof data === 'object' && !(data instanceof Date) && !(data instanceof File)) {
        Object.keys(data).forEach(key => {
            this.buildFormData(formData, data[key], parentKey ? `${parentKey}[${key}]` : key);
        });
    } else {
        const value = data == null ? '' : data;

        formData.append(parentKey, value);
    }
}
function construct_form(data){
    var formData = new FormData();
    if (data && typeof data === 'object' && !(data instanceof Date) && !(data instanceof File)) {
        // data is json, add anti forgery and put into formdata
        addAntiForgeryToken(data);
        for (var key in data) {
            formData.append(key, data[key]);
        } 
    }
    return formData;
}

var createPostRequest = (url, param = {}) => {

    addAntiForgeryToken(param);
    const formData = new FormData();
    buildFormData(formData, param);
    const options = {
        method: 'POST',
        data: formData,
        url: url,
        headers: {
            'x-requested-with': 'XMLHttpRequest'
        }
    };
    return window.axios(options)
        .then(function (result) {
            const response = result.data;
            return response;
        })
        .catch(function (error) {
            console.log(error);
            return Promise.reject(error);
        });
}

var getBlobAsync = async (url, param = {}) => {
    addAntiForgeryToken(param);
    const formData = new FormData();
    buildFormData(formData, param);
    const options = {
        method: 'POST',
        data: formData,
        url: url,
        responseType: 'blob',
    };
    var result = await window.axios(options);
    return result;
}
var showLoading = (element) => {
    document.body.style.overflow = 'hidden';
    $(element).addClass("sw-loading");
}
var hideLoading = (element) => {
    $(element).removeClass("sw-loading");
    document.body.style.overflow = 'visible';
}
var parseFromString = (s) => {
    return (new DOMParser()).parseFromString(s, 'text/html').body.textContent;
}