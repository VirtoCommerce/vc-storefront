/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = "./main.ts");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./app.ts":
/*!****************!*\
  !*** ./app.ts ***!
  \****************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
class App {
    constructor(dispatcher) {
        this.dispatcher = dispatcher;
        this.list = [];
    }
    run() {
        this.reloadResources();
        this.dispatcher.handleMessage =
            (handler, msg) => {
                handler.execute(msg, this.list);
            };
        this.dispatcher.run();
        // ServiceLocator.getMessages().ping();
    }
    getList() {
        return this.list;
    }
    reloadResources() {
        var urlParams = new URLSearchParams(window.location.search);
        var prefix = urlParams.get('preview_mode');
        var suffix = "?preview_mode=" + prefix + "&v=" + (new Date().getTime());
        var nodes = document.getElementsByTagName("link");
        function generateLinkNode(url) {
            var result = document.createElement("link");
            result.setAttribute("rel", "stylesheet");
            result.setAttribute("type", "text/css");
            result.setAttribute("href", url);
            return result;
        }
        for (var i = 0; i < nodes.length; i++) {
            var styleSheet = nodes[i];
            if (styleSheet.href && styleSheet.href.startsWith(document.location.origin) && styleSheet.href.endsWith(".css")) {
                var url = styleSheet.href + suffix;
                var newlink = generateLinkNode(url);
                const parent = styleSheet.parentNode;
                if (!!parent) {
                    parent.replaceChild(newlink, styleSheet);
                }
            }
        }
    }
}
exports.App = App;


/***/ }),

/***/ "./block.view-model.ts":
/*!*****************************!*\
  !*** ./block.view-model.ts ***!
  \*****************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const service_locator_1 = __webpack_require__(/*! ./service-locator */ "./service-locator.ts");
class BlockViewModel {
    constructor() {
        this.onSelect = () => {
            this.eventsDispatcher.selectBlock(this);
        };
        this.onLeave = () => {
            this.eventsDispatcher.unlightBlock();
        };
        this.onHover = () => {
            this.eventsDispatcher.highlightBlock(this);
        };
    }
    get eventsDispatcher() {
        return service_locator_1.ServiceLocator.getDispatcher();
    }
}
exports.BlockViewModel = BlockViewModel;


/***/ }),

/***/ "./dnd.interactor.ts":
/*!***************************!*\
  !*** ./dnd.interactor.ts ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const helpers_1 = __webpack_require__(/*! ./helpers */ "./helpers.ts");
const events_bus_1 = __webpack_require__(/*! ./root/events.bus */ "./root/events.bus.ts");
class DndInteractor {
    constructor(container, listAccessor) {
        this.container = container;
        this.listAccessor = listAccessor;
        this.delta = 10;
        this.startMouseY = null;
        this.oldStyle = {};
        this.onDragStarted = () => { };
        this.onDragFinished = (draggedModel) => { };
        this.onSwapBlocks = (first, second) => { };
        this.placeholder = document.createElement('div');
        this.placeholder.style.backgroundColor = '#eeeeee';
        window.addEventListener('mousemove', ($event) => {
            if (this.isPressed) {
                if (!this.dragStarted) {
                    this.startDrag($event);
                }
                else {
                    this.drag($event);
                }
            }
        });
        window.addEventListener('mouseup', ($event) => {
            if (this.dragStarted) {
                this.releaseDrag();
            }
            this.isPressed = false;
        });
    }
    mouseDown($event, vm) {
        this.isPressed = true;
        this.model = vm;
        this.startMouseY = $event.pageY;
        this.elementRect = helpers_1.measureElement(vm.element);
        this.placeholder.style.height = this.elementRect.height + 'px';
        this.list = this.listAccessor();
        const targetRect = helpers_1.measureElement(this.container);
        this.minY = targetRect.top;
        this.maxY = targetRect.top + targetRect.height;
        this.rects = this.list.map(x => helpers_1.measureElement(x.element));
    }
    startDrag($event) {
        if (Math.abs($event.pageY - this.startMouseY) >= this.delta) {
            this.dragStarted = true;
            this.onDragStarted();
            this.container.replaceChild(this.placeholder, this.model.element);
            document.body.appendChild(this.model.element);
            this.saveStyle();
            this.styleDraggedBlock();
            this.container.style.height = helpers_1.measureElement(this.container).height + 'px';
        }
    }
    drag(event) {
        const mouseY = Math.max(Math.min(event.pageY, this.maxY), this.minY);
        this.model.element.style.top = mouseY - this.startMouseY + this.elementRect.top + 'px';
        const to = this.rects.findIndex(x => x !== this.model && x.top < mouseY && (x.top + x.height) > mouseY);
        const from = this.list.indexOf(this.model);
        if (from === -1 || to === -1 || from === to)
            return;
        const middleY = (this.rects[to].top + this.rects[to].height / 2);
        const needSwap = (from > to && mouseY <= middleY) || (from <= to && mouseY > middleY);
        if (needSwap) {
            console.log(from, to, mouseY, this.rects[to]);
            this.container.removeChild(this.placeholder);
            const beforeElement = this.container.children.item(to);
            this.container.insertBefore(this.placeholder, beforeElement);
            const msg = { content: { id: this.model.id, currentIndex: from, newIndex: to } };
            events_bus_1.EventsBus.Current.publish('dnd.swap-blocks', msg, this);
            console.log('swapped');
            // debugger;
        }
        // get new coords
        // search element under mouse
        // if above a half of element change up (send message to designer)
        // if below a half of element change down (send message to designer)
    }
    releaseDrag() {
        // if drag happens
        // replace placeholder with source element
        // restore height of other elements
        // 'select' should be occured automatically
        if (this.dragStarted) {
            console.log('release drag', this.model);
            document.body.removeChild(this.model.element);
            this.restoreStyles();
            this.container.replaceChild(this.model.element, this.placeholder);
            this.elementRect = null;
            this.startMouseY = null;
            this.onDragFinished(this.model);
            this.oldStyle = {};
            this.container.style.height = '';
        }
        this.dragStarted = false;
        this.isPressed = false;
        this.model = null;
    }
    saveStyle() {
        this.oldStyle.left = this.model.element.style.left;
        this.oldStyle.top = this.model.element.style.top;
        this.oldStyle.width = this.model.element.style.width;
        this.oldStyle.height = this.model.element.style.height;
        this.oldStyle.postion = this.model.element.style.position;
        this.oldStyle.backgroundColor = this.model.element.style.backgroundColor;
        this.oldStyle.border = this.model.element.style.border;
        this.oldStyle.opacity = this.model.element.style.opacity;
    }
    restoreStyles() {
        this.model.element.style.position = this.oldStyle.position || 'static';
        this.model.element.style.left = this.oldStyle.left;
        this.model.element.style.top = this.oldStyle.top;
        this.model.element.style.width = this.oldStyle.width;
        this.model.element.style.height = this.oldStyle.height;
        this.model.element.style.backgroundColor = this.oldStyle.backgroundColor;
        this.model.element.style.border = this.oldStyle.border;
        this.model.element.style.opacity = this.oldStyle.opacity;
    }
    styleDraggedBlock() {
        this.model.element.style.left = this.elementRect.left + 'px';
        this.model.element.style.top = this.elementRect.top + 'px';
        this.model.element.style.width = this.elementRect.width + 'px';
        this.model.element.style.height = this.elementRect.height + 'px';
        this.model.element.style.overflow = 'hidden';
        this.model.element.style.position = 'absolute';
        this.model.element.style.backgroundColor = '#fefefe';
        this.model.element.style.border = '3px solid #33ada9';
        this.model.element.style.opacity = "0.5";
    }
}
exports.DndInteractor = DndInteractor;


/***/ }),

/***/ "./environment.ts":
/*!************************!*\
  !*** ./environment.ts ***!
  \************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.Environment = {
    RenderBlockApiUrl: '/designer-preview/block',
    DesignerUrl: ''
};


/***/ }),

/***/ "./events.dispatcher.ts":
/*!******************************!*\
  !*** ./events.dispatcher.ts ***!
  \******************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const events_bus_1 = __webpack_require__(/*! ./root/events.bus */ "./root/events.bus.ts");
class EventsDispatcher {
    constructor(factory, messages) {
        this.factory = factory;
        this.messages = messages;
        this.handleMessage = () => { };
        events_bus_1.EventsBus.Current.subscribe('dnd.swap-blocks', (args, _source) => {
            this.handleMessage(this.factory.get('swap'), args);
            this.swapBlock(args);
            return null;
        });
    }
    run() {
        window.addEventListener('message', (event) => {
            if (event.data) {
                this.handleEvent(event.data);
            }
        });
    }
    selectBlock(vm) {
        if (!!vm && vm.source) {
            this.handleEvent({ type: 'select', content: vm.source });
            this.messages.selectBlock(vm.source);
            vm.selected = false;
        }
        else {
            this.handleEvent({ type: 'select', content: { id: 0 } });
            this.messages.selectBlock(null);
            if (!!vm) {
                vm.selected = true;
            }
        }
    }
    highlightBlock(vm) {
        this.messages.blockHover(vm.source);
    }
    unlightBlock() {
        this.messages.blockHover({ id: 0 });
    }
    swapBlock(args) {
        this.messages.swapBlocks(args);
    }
    handleEvent(msg) {
        const handler = this.factory.get(msg.type);
        if (handler) {
            this.handleMessage(handler, msg);
        }
    }
}
exports.EventsDispatcher = EventsDispatcher;


/***/ }),

/***/ "./handlers.factory.ts":
/*!*****************************!*\
  !*** ./handlers.factory.ts ***!
  \*****************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const handlers = __webpack_require__(/*! ./handlers */ "./handlers/index.ts");
class HandlersFactory {
    get(key) {
        return HandlersFactory.handlers.find(x => x.key == key);
    }
}
exports.HandlersFactory = HandlersFactory;
HandlersFactory.handlers = [
    new handlers.AddHandler(),
    new handlers.UpdateHandler(),
    new handlers.CloneHandler(),
    new handlers.HideHandler(),
    new handlers.ShowHandler(),
    new handlers.RemoveHandler(),
    new handlers.PreviewHandler(),
    new handlers.SelectHandler(),
    new handlers.SwapHandler(),
    new handlers.ReloadHandler(),
    new handlers.PageHandler(),
    new handlers.HoverHandler()
];


/***/ }),

/***/ "./handlers/add.handler.ts":
/*!*********************************!*\
  !*** ./handlers/add.handler.ts ***!
  \*********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const base_handler_1 = __webpack_require__(/*! ./base.handler */ "./handlers/base.handler.ts");
class AddHandler extends base_handler_1.BaseHandler {
    constructor() {
        super(...arguments);
        this.key = 'add';
    }
    executeInternal(msg, list, vm) {
        this.clearPreview(list);
        list.push(vm);
        vm.selected = true;
        this.reloadBlock(vm.source).then(result => {
            vm.htmlString = result;
            this.renderer.add(vm);
            this.renderer.select(vm);
        });
    }
}
exports.AddHandler = AddHandler;


/***/ }),

/***/ "./handlers/base.handler.ts":
/*!**********************************!*\
  !*** ./handlers/base.handler.ts ***!
  \**********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const block_view_model_1 = __webpack_require__(/*! ../block.view-model */ "./block.view-model.ts");
const service_locator_1 = __webpack_require__(/*! ../service-locator */ "./service-locator.ts");
class BaseHandler {
    get renderer() {
        return service_locator_1.ServiceLocator.getRenderer();
    }
    execute(msg, list) {
        let vm = this.getViewModel(msg.content.id, list);
        if (!vm) {
            vm = this.createViewModel(msg.content);
        }
        this.executeInternal(msg, list, vm);
    }
    executeInternal(msg, list, vm) { }
    reloadBlock(model) {
        return service_locator_1.ServiceLocator.getHttp().post(model);
    }
    generateId(id) {
        if (id) {
            return `instance${id}`;
        }
        return 'preview-instance';
    }
    createViewModel(content, isPreview = false) {
        const result = new block_view_model_1.BlockViewModel();
        Object.assign(result, {
            id: this.generateId(content.id),
            source: content,
            element: null,
            isPreview: isPreview,
            htmlString: null,
            selected: false,
            hidden: !!content.hidden
        });
        return result;
    }
    getViewModel(id, list) {
        const internalId = this.generateId(id);
        return list.find(x => x.id === internalId);
    }
    deselectAll(list) {
        list.forEach(x => x.selected = false);
    }
    clearPreview(list) {
        const listToRemove = list.map((item, index) => ({ item, index })).filter(x => x.item.isPreview);
        listToRemove.forEach(x => {
            list.splice(x.index, 1);
            x.item.element.remove();
        });
    }
}
exports.BaseHandler = BaseHandler;


/***/ }),

/***/ "./handlers/clone.handler.ts":
/*!***********************************!*\
  !*** ./handlers/clone.handler.ts ***!
  \***********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const base_handler_1 = __webpack_require__(/*! ./base.handler */ "./handlers/base.handler.ts");
class CloneHandler extends base_handler_1.BaseHandler {
    constructor() {
        super(...arguments);
        this.key = 'clone';
    }
    execute(msg, list) {
        this.deselectAll(list);
        const source = this.getViewModel(msg.content.source, list);
        const model = Object.assign(Object.assign({}, source.source), { id: msg.content.destination });
        const clone = this.createViewModel(model);
        clone.htmlString = source.htmlString;
        clone.hidden = source.hidden;
        clone.selected = true;
        const index = list.indexOf(source);
        list.splice(index + 1, 0, clone);
        this.renderer.insert(clone, index + 1);
        this.renderer.select(clone);
        setTimeout(() => {
            this.renderer.scrollTo(clone);
        }, 300);
    }
}
exports.CloneHandler = CloneHandler;


/***/ }),

/***/ "./handlers/hide.handler.ts":
/*!**********************************!*\
  !*** ./handlers/hide.handler.ts ***!
  \**********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const base_handler_1 = __webpack_require__(/*! ./base.handler */ "./handlers/base.handler.ts");
class HideHandler extends base_handler_1.BaseHandler {
    constructor() {
        super(...arguments);
        this.key = 'hide';
    }
    executeInternal(msg, list, vm) {
        vm.hidden = true;
        vm.element.style.display = 'none';
    }
}
exports.HideHandler = HideHandler;


/***/ }),

/***/ "./handlers/hover.handler.ts":
/*!***********************************!*\
  !*** ./handlers/hover.handler.ts ***!
  \***********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const base_handler_1 = __webpack_require__(/*! ./base.handler */ "./handlers/base.handler.ts");
class HoverHandler extends base_handler_1.BaseHandler {
    constructor() {
        super(...arguments);
        this.key = 'hover';
    }
    execute(msg, list) {
        this.deselectAll(list);
        const content = msg.content;
        if (!content.id) {
            this.renderer.hover();
        }
        else {
            super.execute(msg, list);
        }
    }
    executeInternal(msg, list, vm) {
        this.renderer.hover(vm);
    }
}
exports.HoverHandler = HoverHandler;


/***/ }),

/***/ "./handlers/index.ts":
/*!***************************!*\
  !*** ./handlers/index.ts ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

function __export(m) {
    for (var p in m) if (!exports.hasOwnProperty(p)) exports[p] = m[p];
}
Object.defineProperty(exports, "__esModule", { value: true });
__export(__webpack_require__(/*! ./add.handler */ "./handlers/add.handler.ts"));
__export(__webpack_require__(/*! ./clone.handler */ "./handlers/clone.handler.ts"));
__export(__webpack_require__(/*! ./hide.handler */ "./handlers/hide.handler.ts"));
__export(__webpack_require__(/*! ./hover.handler */ "./handlers/hover.handler.ts"));
__export(__webpack_require__(/*! ./preview.handler */ "./handlers/preview.handler.ts"));
__export(__webpack_require__(/*! ./remove.handler */ "./handlers/remove.handler.ts"));
__export(__webpack_require__(/*! ./select.handler */ "./handlers/select.handler.ts"));
__export(__webpack_require__(/*! ./show.handler */ "./handlers/show.handler.ts"));
__export(__webpack_require__(/*! ./swap.handler */ "./handlers/swap.handler.ts"));
__export(__webpack_require__(/*! ./update.handler */ "./handlers/update.handler.ts"));
__export(__webpack_require__(/*! ./reload.handler */ "./handlers/reload.handler.ts"));
__export(__webpack_require__(/*! ./page.handler */ "./handlers/page.handler.ts"));


/***/ }),

/***/ "./handlers/page.handler.ts":
/*!**********************************!*\
  !*** ./handlers/page.handler.ts ***!
  \**********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const service_locator_1 = __webpack_require__(/*! ./../service-locator */ "./service-locator.ts");
const base_handler_1 = __webpack_require__(/*! ./base.handler */ "./handlers/base.handler.ts");
class PageHandler extends base_handler_1.BaseHandler {
    constructor() {
        super(...arguments);
        this.key = 'page';
    }
    execute(msg, list) {
        console.log(msg);
        const blocks = msg.content.blocks;
        blocks.forEach(x => {
            const vm = this.createViewModel(x);
            list.push(vm);
        });
        Promise.all(list.map(x => {
            const promise = this.reloadBlock(x.source).then(html => {
                x.htmlString = html;
                return html;
            });
            return promise;
        })).then(() => {
            list.forEach(x => {
                this.renderer.add(x);
            });
            // var $: any = window['jQuery'];
            // $(".carousel-block").carousel();
            service_locator_1.ServiceLocator.getMessages().renderComplete();
        });
    }
}
exports.PageHandler = PageHandler;


/***/ }),

/***/ "./handlers/preview.handler.ts":
/*!*************************************!*\
  !*** ./handlers/preview.handler.ts ***!
  \*************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const base_handler_1 = __webpack_require__(/*! ./base.handler */ "./handlers/base.handler.ts");
class PreviewHandler extends base_handler_1.BaseHandler {
    constructor() {
        super(...arguments);
        this.key = 'preview';
    }
    execute(msg, list) {
        this.clearPreview(list);
        if (!!msg.content) {
            const vm = this.createViewModel(msg.content, true);
            list.push(vm);
            this.reloadBlock(vm.source).then(result => {
                vm.htmlString = result;
                this.renderer.add(vm);
                this.renderer.scrollTo(vm);
            });
        }
    }
}
exports.PreviewHandler = PreviewHandler;


/***/ }),

/***/ "./handlers/reload.handler.ts":
/*!************************************!*\
  !*** ./handlers/reload.handler.ts ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const base_handler_1 = __webpack_require__(/*! ./base.handler */ "./handlers/base.handler.ts");
class ReloadHandler extends base_handler_1.BaseHandler {
    constructor() {
        super(...arguments);
        this.key = 'reload';
    }
    execute(msg, list) {
        document.location.reload();
    }
}
exports.ReloadHandler = ReloadHandler;


/***/ }),

/***/ "./handlers/remove.handler.ts":
/*!************************************!*\
  !*** ./handlers/remove.handler.ts ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const base_handler_1 = __webpack_require__(/*! ./base.handler */ "./handlers/base.handler.ts");
class RemoveHandler extends base_handler_1.BaseHandler {
    constructor() {
        super(...arguments);
        this.key = 'remove';
    }
    executeInternal(msg, list, vm) {
        const index = list.indexOf(vm);
        list.splice(index, 1);
        vm.element.remove();
        this.renderer.select();
    }
}
exports.RemoveHandler = RemoveHandler;


/***/ }),

/***/ "./handlers/select.handler.ts":
/*!************************************!*\
  !*** ./handlers/select.handler.ts ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const base_handler_1 = __webpack_require__(/*! ./base.handler */ "./handlers/base.handler.ts");
class SelectHandler extends base_handler_1.BaseHandler {
    constructor() {
        super(...arguments);
        this.key = 'select';
    }
    execute(msg, list) {
        this.deselectAll(list);
        this.clearPreview(list);
        const content = msg.content;
        if (content.id === 0) {
            this.renderer.select();
        }
        else {
            super.execute(msg, list);
        }
    }
    executeInternal(msg, list, vm) {
        vm.selected = true;
        this.renderer.select(vm);
        this.renderer.scrollTo(vm);
    }
}
exports.SelectHandler = SelectHandler;


/***/ }),

/***/ "./handlers/show.handler.ts":
/*!**********************************!*\
  !*** ./handlers/show.handler.ts ***!
  \**********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const base_handler_1 = __webpack_require__(/*! ./base.handler */ "./handlers/base.handler.ts");
class ShowHandler extends base_handler_1.BaseHandler {
    constructor() {
        super(...arguments);
        this.key = 'show';
    }
    executeInternal(msg, list, vm) {
        vm.hidden = false;
        vm.element.style.display = 'block';
        this.renderer.scrollTo(vm);
    }
}
exports.ShowHandler = ShowHandler;


/***/ }),

/***/ "./handlers/swap.handler.ts":
/*!**********************************!*\
  !*** ./handlers/swap.handler.ts ***!
  \**********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const base_handler_1 = __webpack_require__(/*! ./base.handler */ "./handlers/base.handler.ts");
class SwapHandler extends base_handler_1.BaseHandler {
    constructor() {
        super(...arguments);
        this.key = 'swap';
    }
    execute(msg, list) {
        const vm = list[msg.content.currentIndex];
        list.splice(msg.content.currentIndex, 1);
        list.splice(msg.content.newIndex, 0, vm);
        if (list[msg.content.currentIndex].element.parentElement === list[msg.content.newIndex].element.parentElement) {
            vm.element.remove();
            this.renderer.insert(vm, msg.content.newIndex);
        }
    }
}
exports.SwapHandler = SwapHandler;


/***/ }),

/***/ "./handlers/update.handler.ts":
/*!************************************!*\
  !*** ./handlers/update.handler.ts ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const base_handler_1 = __webpack_require__(/*! ./base.handler */ "./handlers/base.handler.ts");
class UpdateHandler extends base_handler_1.BaseHandler {
    constructor() {
        super(...arguments);
        this.key = 'update';
    }
    executeInternal(msg, list, vm) {
        vm.source = msg.content;
        this.reloadBlock(vm.source).then((result) => {
            vm.htmlString = result;
            this.renderer.update(vm);
            this.renderer.select(vm);
            // var $: any = window['jQuery'];
            // $(".carousel-block").carousel();
        });
    }
}
exports.UpdateHandler = UpdateHandler;


/***/ }),

/***/ "./helpers.ts":
/*!********************!*\
  !*** ./helpers.ts ***!
  \********************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
function measureElement(element) {
    const target = element;
    const target_width = target.offsetWidth;
    const target_height = target.offsetHeight;
    let rect = {};
    let gleft = 0;
    let gtop = 0;
    var moonwalk = function (_parent) {
        if (!!_parent) {
            gleft += _parent.offsetLeft;
            gtop += _parent.offsetTop;
            moonwalk(_parent.offsetParent);
        }
        else {
            return rect = {
                top: target.offsetTop + gtop,
                left: target.offsetLeft + gleft,
                height: target_height,
                width: target_width
            };
        }
    };
    moonwalk(target.offsetParent);
    return rect;
}
exports.measureElement = measureElement;


/***/ }),

/***/ "./main.ts":
/*!*****************!*\
  !*** ./main.ts ***!
  \*****************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const environment_1 = __webpack_require__(/*! ./environment */ "./environment.ts");
const service_locator_1 = __webpack_require__(/*! ./service-locator */ "./service-locator.ts");
document.addEventListener('DOMContentLoaded', () => {
    console.log('run preview window');
    environment_1.Environment.DesignerUrl = window['__designer_preview__'];
    const app = service_locator_1.ServiceLocator.createApp();
    app.run();
});
window.addEventListener('click', (event) => {
    event.preventDefault();
});


/***/ }),

/***/ "./preview.interactor.ts":
/*!*******************************!*\
  !*** ./preview.interactor.ts ***!
  \*******************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const service_locator_1 = __webpack_require__(/*! ./service-locator */ "./service-locator.ts");
const helpers_1 = __webpack_require__(/*! ./helpers */ "./helpers.ts");
class PreviewInteractor {
    constructor(dnd) {
        this.dnd = dnd;
        this.borderWidth = 3;
        this.hoveredViewModel = null;
        this.selectedViewModel = null;
        this.inactive = false; // use with dnd
        this.createHoverElement();
        this.createSelectElement();
        this.dnd.onDragStarted = () => {
            this.inactive = true;
            this.hideHoverElement();
            this.hideSelectElement();
        };
        this.dnd.onDragFinished = () => {
            this.inactive = false;
        };
    }
    hover(vm) {
        if (this.inactive)
            return;
        if (vm == null || this.selectedViewModel == vm) {
            this.hideHoverElement();
        }
        else {
            this.hoveredViewModel = vm;
            this.hoverElement.style.display = 'block';
            this.placeElementHover(vm.element, this.hoverElement);
        }
    }
    select(vm) {
        if (this.inactive)
            return;
        this.selectedViewModel = vm;
        this.selectElement.style.display = 'block';
        this.placeElementHover(vm.element, this.selectElement);
    }
    deselect() {
        this.hideSelectElement();
    }
    scrollTo(vm) {
        if (this.inactive)
            return;
        const rect = helpers_1.measureElement(vm.element);
        const targetPosition = rect.top - window.innerHeight / 10;
        window.scroll({
            top: targetPosition,
            behavior: 'smooth'
        });
    }
    hideSelectElement() {
        this.selectedViewModel = null;
        this.selectElement.style.display = 'none';
    }
    hideHoverElement() {
        this.hoveredViewModel = null;
        this.hoverElement.style.display = 'none';
    }
    createHoverElement() {
        const result = this.createShadowElement();
        result.style.border = `${this.borderWidth}px dotted #33ada9`;
        result.addEventListener('mouseleave', () => {
            if (this.hoveredViewModel != null) {
                this.hoveredViewModel.onLeave();
            }
            this.hideHoverElement();
        });
        result.addEventListener('click', (event) => {
            this.select(this.hoveredViewModel);
            this.hoveredViewModel.onSelect();
            this.hideHoverElement();
        });
        result.addEventListener('mousedown', (event) => {
            this.dnd.mouseDown(event, this.hoveredViewModel);
        });
        this.hoverElement = result;
        return result;
    }
    createSelectElement() {
        const result = this.createShadowElement();
        result.style.border = `${this.borderWidth}px solid #33ada9`;
        result.addEventListener('click', (event) => {
            const dispatcher = service_locator_1.ServiceLocator.getDispatcher();
            dispatcher.selectBlock(null);
        });
        result.addEventListener('mousedown', (event) => {
            this.dnd.mouseDown(event, this.selectedViewModel);
        });
        this.selectElement = result;
        return result;
    }
    createShadowElement() {
        const result = document.createElement('div');
        result.style.position = 'absolute';
        result.style.display = 'none';
        result.style.zIndex = '10000';
        document.body.appendChild(result);
        return result;
    }
    placeElementHover(source, target) {
        if (!source) {
            return;
        }
        const rect = helpers_1.measureElement(source);
        const doubleWidth = this.borderWidth * 2;
        const delta = 0;
        target.style.top = (rect.top + delta) + 'px';
        target.style.left = (rect.left + delta) + 'px';
        target.style.height = (rect.height - 2 * delta) + 'px';
        target.style.width = (rect.width - 2 * delta) + 'px';
        target.style.display = 'block';
    }
}
exports.PreviewInteractor = PreviewInteractor;


/***/ }),

/***/ "./renderer.ts":
/*!*********************!*\
  !*** ./renderer.ts ***!
  \*********************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const service_locator_1 = __webpack_require__(/*! ./service-locator */ "./service-locator.ts");
class Renderer {
    constructor(container) {
        this.container = container;
        this.interactor = service_locator_1.ServiceLocator.getPreviewInteractor();
    }
    add(vm) {
        vm.element = this.createElement(vm);
        this.container.append(vm.element);
        if (vm.hidden) {
            vm.element.style.display = 'none';
        }
    }
    update(vm) {
        const element = vm.element;
        if (!element) {
            return;
        }
        vm.element = this.createElement(vm);
        this.container.replaceChild(vm.element, element);
        this.interactor.select(vm);
        this.hover(vm);
    }
    insert(vm, index) {
        vm.element = this.createElement(vm);
        const beforeElement = this.container.children.item(index);
        this.container.insertBefore(vm.element, beforeElement);
        if (vm.hidden) {
            vm.element.style.display = 'none';
        }
    }
    select(vm = null) {
        if (vm === null || vm.hidden || !vm.selected) {
            this.interactor.deselect();
        }
        else {
            this.interactor.select(vm);
        }
    }
    scrollTo(vm) {
        if (vm && vm.element && !vm.hidden) {
            this.interactor.scrollTo(vm);
        }
    }
    hover(vm = null) {
        if (vm === null || vm.hidden || vm.selected) {
            this.interactor.hover(null);
        }
        else {
            this.interactor.hover(vm);
        }
    }
    createElement(vm) {
        const div = document.createElement('div');
        div.innerHTML = `<div>${vm.htmlString}</div>`;
        const result = div.firstChild;
        result.style.userSelect = 'none';
        if (!vm.isPreview) {
            result.addEventListener('mouseover', ($event) => {
                this.interactor.hover(vm);
                vm.onHover();
            });
            result.addEventListener('click', () => vm.onSelect());
        }
        return result;
    }
}
exports.Renderer = Renderer;


/***/ }),

/***/ "./root/events.bus.ts":
/*!****************************!*\
  !*** ./root/events.bus.ts ***!
  \****************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
class EventsBus {
    constructor() {
        this.subscribers = {};
    }
    publish(type, args, source) {
        if (this.subscribers[type]) {
            this.subscribers[type].forEach(handler => {
                handler(args, source);
            });
        }
    }
    subscribe(type, handler) {
        if (!this.subscribers[type]) {
            this.subscribers[type] = [];
        }
        this.subscribers[type].push(handler);
        return () => {
            const index = this.subscribers[type].indexOf(handler);
            if (index !== -1) {
                this.subscribers[type].splice(index, 1);
            }
        };
    }
}
exports.EventsBus = EventsBus;
EventsBus.Current = new EventsBus();


/***/ }),

/***/ "./service-locator.ts":
/*!****************************!*\
  !*** ./service-locator.ts ***!
  \****************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const dnd_interactor_1 = __webpack_require__(/*! ./dnd.interactor */ "./dnd.interactor.ts");
const preview_interactor_1 = __webpack_require__(/*! ./preview.interactor */ "./preview.interactor.ts");
const events_bus_1 = __webpack_require__(/*! ./root/events.bus */ "./root/events.bus.ts");
const http_service_1 = __webpack_require__(/*! ./services/http.service */ "./services/http.service.ts");
const events_dispatcher_1 = __webpack_require__(/*! ./events.dispatcher */ "./events.dispatcher.ts");
const renderer_1 = __webpack_require__(/*! ./renderer */ "./renderer.ts");
const handlers_factory_1 = __webpack_require__(/*! ./handlers.factory */ "./handlers.factory.ts");
const environment_1 = __webpack_require__(/*! ./environment */ "./environment.ts");
const app_1 = __webpack_require__(/*! ./app */ "./app.ts");
const messages_service_1 = __webpack_require__(/*! ./services/messages.service */ "./services/messages.service.ts");
class ServiceLocator {
    static createApp() {
        this._container = document.getElementById("designer-preview");
        return this._app || (this._app = new app_1.App(this.getDispatcher()));
    }
    static getPreviewInteractor() {
        if (!this._interactor) {
            this._interactor = new preview_interactor_1.PreviewInteractor(this.getDndInteractor());
        }
        return this._interactor;
    }
    static getDndInteractor() {
        return this._dnd || (this._dnd = new dnd_interactor_1.DndInteractor(this._container, () => this._app.getList()));
    }
    static getEventBus() {
        return this._eventBus || (this._eventBus = new events_bus_1.EventsBus());
    }
    static getHttp() {
        return this._http || (this._http = new http_service_1.HttpService(environment_1.Environment.RenderBlockApiUrl));
    }
    static getMessages() {
        return this._messages || (this._messages = new messages_service_1.MessagesService(environment_1.Environment.DesignerUrl));
    }
    static getRenderer() {
        return this._renderer || (this._renderer = new renderer_1.Renderer(this._container));
    }
    static getFactory() {
        return this._factory || (this._factory = new handlers_factory_1.HandlersFactory());
    }
    static getDispatcher() {
        return this._dispatcher || (this._dispatcher = new events_dispatcher_1.EventsDispatcher(this.getFactory(), this.getMessages()));
    }
}
exports.ServiceLocator = ServiceLocator;


/***/ }),

/***/ "./services/http.service.ts":
/*!**********************************!*\
  !*** ./services/http.service.ts ***!
  \**********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
class HttpService {
    constructor(endpoint) {
        this.endpoint = endpoint;
    }
    get() {
    }
    postTo(endpoint, model) {
        // https://gist.github.com/codecorsair/e14ec90cee91fa8f56054afaa0a39f13
        return new Promise((resolve, reject) => {
            const xhr = new XMLHttpRequest();
            xhr.open('post', endpoint);
            xhr.setRequestHeader('Accept', 'application/json, text/javascript, text/plain');
            xhr.setRequestHeader('Cache-Control', 'no-cache');
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.send(JSON.stringify(model));
            // xhr.timeout = timeout;
            xhr.onload = evt => {
                // const result = {
                //     ok: xhr.status >= 200 && xhr.status < 300,
                //     status: xhr.status,
                //     statusText: xhr.statusText,
                //     headers: xhr.getAllResponseHeaders(),
                //     data: xhr.responseText
                // };
                resolve(xhr.responseText.trim());
                // this.blocks.push(model); or replace
            };
            // xhr.onerror = evt => {
            //   resolve(errorResponse(xhr, 'Failed to make request.'));
            // };
            // xhr.ontimeout = evt => {
            //   resolve(errorResponse(xhr, 'Request took longer than expected.'));
            // };
        });
    }
    post(model) {
        return this.postTo(this.endpoint, model);
    }
}
exports.HttpService = HttpService;


/***/ }),

/***/ "./services/messages.service.ts":
/*!**************************************!*\
  !*** ./services/messages.service.ts ***!
  \**************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
class MessagesService {
    constructor(parentOrigin) {
        this.parentOrigin = parentOrigin;
    }
    renderComplete() {
        this.send('render-complete', null);
    }
    blockHover(model) {
        this.send('hover', { id: model.id });
    }
    swapBlocks(args) {
        this.send('swap', Object.assign({ type: 'swap' }, args));
    }
    selectBlock(model) {
        this.send('select', model ? { id: model.id } : null);
    }
    ping() {
        this.send('ping', null);
    }
    send(message, model) {
        const msg = Object.assign({ type: message }, model);
        console.log('send to designer', msg);
        window.parent.postMessage(msg, this.parentOrigin);
    }
}
exports.MessagesService = MessagesService;


/***/ })

/******/ });
//# sourceMappingURL=designer.bundle.js.map