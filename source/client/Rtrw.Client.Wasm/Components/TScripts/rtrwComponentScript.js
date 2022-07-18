// Element Reference
class RtrwElementReference {
    constructor() {
        this.listenerId = 0;
        this.eventListeners = {};
    }

    focus(element) {
        if (element) {
            element.focus();
        }
    }

    focusFirst(element, skip = 0, min = 0) {
        if (element) {
            let tabbables = getTabbableElements(element);
            if (tabbables.length <= min)
                element.focus();
            else
                tabbables[skip].focus();
        }
    }

    focusLast(element, skip = 0, min = 0) {
        if (element) {
            let tabbables = getTabbableElements(element);
            if (tabbables.length <= min)
                element.focus();
            else
                tabbables[tabbables.length - skip - 1].focus();
        }
    }

    saveFocus(element) {
        if (element) {
            element['rtrw_savedFocus'] = document.activeElement;
        }
    }

    restoreFocus(element) {
        if (element) {
            let previous = element['rtrw_savedFocus'];
            delete element['rtrw_savedFocus']
            if (previous)
                previous.focus();
        }
    }

    selectRange(element, pos1, pos2) {
        if (element) {
            if (element.createTextRange) {
                let selRange = element.createTextRange();
                selRange.collapse(true);
                selRange.moveStart('character', pos1);
                selRange.moveEnd('character', pos2);
                selRange.select();
            } else if (element.setSelectionRange) {
                element.setSelectionRange(pos1, pos2);
            } else if (element.selectionStart) {
                element.selectionStart = pos1;
                element.selectionEnd = pos2;
            }
            element.focus();
        }
    }

    select(element) {
        if (element) {
            element.select();
        }
    }
    /**
     * gets the client rect of the parent of the element
     * @param {HTMLElement} element
     */
    getClientRectFromParent(element) {
        if (!element) return;
        let parent = element.parentElement;
        if (!parent) return;
        return this.getBoundingClientRect(parent);
    }

    /**
     * Gets the client rect of the first child of the element
     * @param {any} element
     */

    getClientRectFromFirstChild(element) {
        if (!element) return;
        let child = element.children && element.children[0];
        if (!child) return;
        return this.getBoundingClientRect(child);
    }


    getBoundingClientRect(element) {
        if (!element) return;

        var rect = JSON.parse(JSON.stringify(element.getBoundingClientRect()));

        rect.scrollY = window.scrollY || document.documentElement.scrollTop;
        rect.scrollX = window.scrollX || document.documentElement.scrollLeft;

        rect.windowHeight = window.innerHeight;
        rect.windowWidth = window.innerWidth;
        return rect;
    }

    /**
     * Returns true if the element has any ancestor with style position==="fixed"
     * @param {Element} element
     */
    hasFixedAncestors(element) {
        for (; element && element !== document; element = element.parentNode) {
            if (window.getComputedStyle(element).getPropertyValue("position") === "fixed")
                return true;
        }
        return false
    };

    changeCss(element, css) {
        if (element) {
            element.className = css;
        }
    }

    changeCssVariable(element, name, newValue) {
        if (element) {
            element.style.setProperty(name, newValue);
        }
    }

    addEventListener(element, dotnet, event, callback, spec, stopPropagation) {
        let listener = function (e) {
            const args = Array.from(spec, x => serializeParameter(e, x));
            dotnet.invokeMethodAsync(callback, ...args);
            if (stopPropagation) {
                e.stopPropagation();
            }
        };
        element.addEventListener(event, listener);
        this.eventListeners[++this.listenerId] = listener;
        return this.listenerId;
    }

    removeEventListener(element, event, eventId) {
        element.removeEventListener(event, this.eventListeners[eventId]);
        delete this.eventListeners[eventId];
    }
};
window.rtrwElementRef = new RtrwElementReference();

class RtrwKeyInterceptorFactory {

    connect(dotNetRef, elementId, options) {
        //console.log('[RtrwBlazor | RtrwKeyInterceptorFactory] connect ', { dotNetRef, element, options });
        if (!elementId)
            throw "elementId: expected element id!";
        var element = document.getElementById(elementId);
        if (!element)
            throw "no element found for id: " + elementId;
        if (!element.rtrwKeyInterceptor)
            element.rtrwKeyInterceptor = new RtrwKeyInterceptor(dotNetRef, options);
        element.rtrwKeyInterceptor.connect(element);
    }

    updatekey(elementId, option) {
        var element = document.getElementById(elementId);
        if (!element || !element.rtrwKeyInterceptor)
            return;
        element.rtrwKeyInterceptor.updatekey(option);
    }

    disconnect(elementId) {
        var element = document.getElementById(elementId);
        if (!element || !element.rtrwKeyInterceptor)
            return;
        element.rtrwKeyInterceptor.disconnect();
    }
}
window.rtrwKeyInterceptor = new RtrwKeyInterceptorFactory();


class RtrwKeyInterceptor {

    constructor(dotNetRef, options) {
        this._dotNetRef = dotNetRef;
        this._options = options;
        this.logger = options.enableLogging ? console.log : (message) => { };
        this.logger('[RtrwBlazor | KeyInterceptor] Interceptor initialized', { options });
    }


    connect(element) {
        if (!this._options)
            return;
        if (!this._options.keys)
            throw "_options.keys: array of KeyOptions expected";
        if (!this._options.targetClass)
            throw "_options.targetClass: css class name expected";
        if (this._observer) {
            // don't do double registration
            return;
        }
        var targetClass = this._options.targetClass;
        this.logger('[RtrwBlazor | KeyInterceptor] Start observing DOM of element for changes to child with class ', { element, targetClass });
        this._element = element;
        this._observer = new MutationObserver(this.onDomChanged);
        this._observer.rtrwKeyInterceptor = this;
        this._observer.observe(this._element, { attributes: false, childList: true, subtree: true });
        this._observedChildren = [];
        // transform key options into a key lookup
        this._keyOptions = {};
        this._regexOptions = [];
        for (const keyOption of this._options.keys) {
            if (!keyOption || !keyOption.key) {
                this.logger('[RtrwBlazor | KeyInterceptor] got invalid key options: ', keyOption);
                continue;
            }
            this.setKeyOption(keyOption)
        }
        this.logger('[RtrwBlazor | KeyInterceptor] key options: ', this._keyOptions);
        if (this._regexOptions.size > 0)
            this.logger('[RtrwBlazor | KeyInterceptor] regex options: ', this._regexOptions);
        // register handlers
        for (const child of this._element.getElementsByClassName(targetClass)) {
            this.attachHandlers(child);
        }
    }

    setKeyOption(keyOption) {
        if (keyOption.key.length > 2 && keyOption.key.startsWith('/') && keyOption.key.endsWith('/')) {
            // JS regex key options such as "/[a-z]/" or "/a|b/" but NOT "/[a-z]/g" or "/[a-z]/i"
            keyOption.regex = new RegExp(keyOption.key.substring(1, keyOption.key.length - 1)); // strip the / from start and end
            this._regexOptions.push(keyOption);
        }
        else
            this._keyOptions[keyOption.key.toLowerCase()] = keyOption;
        // remove whitespace and enforce lowercase
        var whitespace = new RegExp("\\s", "g");
        keyOption.preventDown = (keyOption.preventDown || "none").replace(whitespace, "").toLowerCase();
        keyOption.preventUp = (keyOption.preventUp || "none").replace(whitespace, "").toLowerCase();
        keyOption.stopDown = (keyOption.stopDown || "none").replace(whitespace, "").toLowerCase();
        keyOption.stopUp = (keyOption.stopUp || "none").replace(whitespace, "").toLowerCase();
    }

    updatekey(updatedOption) {
        var option = this._keyOptions[updatedOption.key.toLowerCase()];
        option || this.logger('[RtrwBlazor | KeyInterceptor] updating option failed: key not registered');
        this.setKeyOption(updatedOption);
        this.logger('[RtrwBlazor | KeyInterceptor] updated option ', { option, updatedOption });
    }

    disconnect() {
        if (!this._observer)
            return;
        this.logger('[RtrwBlazor | KeyInterceptor] disconnect mutation observer and event handlers');
        this._observer.disconnect();
        this._observer = null;
        for (const child of this._observedChildren)
            this.detachHandlers(child);
    }

    attachHandlers(child) {
        this.logger('[RtrwBlazor | KeyInterceptor] attaching handlers ', { child });
        if (this._observedChildren.indexOf(child) > -1) {
            //console.log("... already attached");
            return;
        }
        child.rtrwKeyInterceptor = this;
        child.addEventListener('keydown', this.onKeyDown);
        child.addEventListener('keyup', this.onKeyUp);
        this._observedChildren.push(child);
    }

    detachHandlers(child) {
        this.logger('[RtrwBlazor | KeyInterceptor] detaching handlers ', { child });
        child.removeEventListener('keydown', this.onKeyDown);
        child.removeEventListener('keyup', this.onKeyUp);
        this._observedChildren = this._observedChildren.filter(x => x !== child);
    }

    onDomChanged(mutationsList, observer) {
        var self = this.rtrwKeyInterceptor; // func is invoked with this == _observer
        //self.logger('[RtrwBlazor | KeyInterceptor] onDomChanged: ', { self });
        var targetClass = self._options.targetClass;
        for (const mutation of mutationsList) {
            //self.logger('[RtrwBlazor | KeyInterceptor] Subtree mutation: ', { mutation });
            for (const element of mutation.addedNodes) {
                if (element.classList && element.classList.contains(targetClass))
                    self.attachHandlers(element);
            }
            for (const element of mutation.removedNodes) {
                if (element.classList && element.classList.contains(targetClass))
                    self.detachHandlers(element);
            }
        }
    }

    matchesKeyCombination(option, args) {
        if (!option || option === "none")
            return false;
        if (option === "any")
            return true;
        var shift = args.shiftKey;
        var ctrl = args.ctrlKey;
        var alt = args.altKey;
        var meta = args.metaKey;
        var any = shift || ctrl || alt || meta;
        if (any && option === "key+any")
            return true;
        if (!any && option.includes("key+none"))
            return true;
        if (!any)
            return false;
        var combi = `key${shift ? "+shift" : ""}${ctrl ? "+ctrl" : ""}${alt ? "+alt" : ""}${meta ? "+meta" : ""}`;
        return option.includes(combi);
    }

    onKeyDown(args) {
        var self = this.rtrwKeyInterceptor; // func is invoked with this == child
        var key = args.key.toLowerCase();
        self.logger('[RtrwBlazor | KeyInterceptor] down "' + key + '"', args);
        var invoke = false;
        if (self._keyOptions.hasOwnProperty(key)) {
            var keyOptions = self._keyOptions[key];
            self.logger('[RtrwBlazor | KeyInterceptor] options for "' + key + '"', keyOptions);
            self.processKeyDown(args, keyOptions);
            if (keyOptions.subscribeDown)
                invoke = true;
        }
        for (const keyOptions of self._regexOptions) {
            if (keyOptions.regex.test(key)) {
                self.logger('[RtrwBlazor | KeyInterceptor] regex options for "' + key + '"', keyOptions);
                self.processKeyDown(args, keyOptions);
                if (keyOptions.subscribeDown)
                    invoke = true;
            }
        }
        if (invoke) {
            var eventArgs = self.toKeyboardEventArgs(args);
            eventArgs.Type = "keydown";
            // we'd like to pass a reference to the child element back to dotnet but we can't
            // https://github.com/dotnet/aspnetcore/issues/16110
            // if we ever need it we'll pass the id up and users need to id the observed elements
            self._dotNetRef.invokeMethodAsync('OnKeyDown', eventArgs);
        }
    }

    processKeyDown(args, keyOptions) {
        if (this.matchesKeyCombination(keyOptions.preventDown, args))
            args.preventDefault();
        if (this.matchesKeyCombination(keyOptions.stopDown, args))
            args.stopPropagation();
    }

    onKeyUp(args) {
        var self = this.rtrwKeyInterceptor; // func is invoked with this == child
        var key = args.key.toLowerCase();
        self.logger('[RtrwBlazor | KeyInterceptor] up "' + key + '"', args);
        var invoke = false;
        if (self._keyOptions.hasOwnProperty(key)) {
            var keyOptions = self._keyOptions[key];
            self.processKeyUp(args, keyOptions);
            if (keyOptions.subscribeUp)
                invoke = true;
        }
        for (const keyOptions of self._regexOptions) {
            if (keyOptions.regex.test(key)) {
                self.processKeyUp(args, keyOptions);
                if (keyOptions.subscribeUp)
                    invoke = true;
            }
        }
        if (invoke) {
            var eventArgs = self.toKeyboardEventArgs(args);
            eventArgs.Type = "keyup";
            // we'd like to pass a reference to the child element back to dotnet but we can't
            // https://github.com/dotnet/aspnetcore/issues/16110
            // if we ever need it we'll pass the id up and users need to id the observed elements
            self._dotNetRef.invokeMethodAsync('OnKeyUp', eventArgs);
        }
    }

    processKeyUp(args, keyOptions) {
        if (this.matchesKeyCombination(keyOptions.preventUp, args))
            args.preventDefault();
        if (this.matchesKeyCombination(keyOptions.stopUp, args))
            args.stopPropagation();
    }

    toKeyboardEventArgs(args) {
        return {
            Key: args.key,
            Code: args.code,
            Location: args.location,
            Repeat: args.repeat,
            CtrlKey: args.ctrlKey,
            ShiftKey: args.shiftKey,
            AltKey: args.altKey,
            MetaKey: args.metaKey
        };
    }

}

window.rtrwPopoverHelper = {
    calculatePopoverPosition: function (list, boundingRect, selfRect) {
        let top = 0;
        let left = 0;
        if (list.indexOf('rtrw-popover-anchor-top-left') >= 0) {
            left = boundingRect.left;
            top = boundingRect.top;
        } else if (list.indexOf('rtrw-popover-anchor-top-center') >= 0) {
            left = boundingRect.left + boundingRect.width / 2;
            top = boundingRect.top;
        } else if (list.indexOf('rtrw-popover-anchor-top-right') >= 0) {
            left = boundingRect.left + boundingRect.width;
            top = boundingRect.top;

        } else if (list.indexOf('rtrw-popover-anchor-center-left') >= 0) {
            left = boundingRect.left;
            top = boundingRect.top + boundingRect.height / 2;
        } else if (list.indexOf('rtrw-popover-anchor-center-center') >= 0) {
            left = boundingRect.left + boundingRect.width / 2;
            top = boundingRect.top + boundingRect.height / 2;
        } else if (list.indexOf('rtrw-popover-anchor-center-right') >= 0) {
            left = boundingRect.left + boundingRect.width;
            top = boundingRect.top + boundingRect.height / 2;

        } else if (list.indexOf('rtrw-popover-anchor-bottom-left') >= 0) {
            left = boundingRect.left;
            top = boundingRect.top + boundingRect.height;
        } else if (list.indexOf('rtrw-popover-anchor-bottom-center') >= 0) {
            left = boundingRect.left + boundingRect.width / 2;
            top = boundingRect.top + boundingRect.height;
        } else if (list.indexOf('rtrw-popover-anchor-bottom-right') >= 0) {
            left = boundingRect.left + boundingRect.width;
            top = boundingRect.top + boundingRect.height;
        }

        let offsetX = 0;
        let offsetY = 0;

        if (list.indexOf('rtrw-popover-top-left') >= 0) {
            offsetX = 0;
            offsetY = 0;
        } else if (list.indexOf('rtrw-popover-top-center') >= 0) {
            offsetX = -selfRect.width / 2;
            offsetY = 0;
        } else if (list.indexOf('rtrw-popover-top-right') >= 0) {
            offsetX = -selfRect.width;
            offsetY = 0;
        }

        else if (list.indexOf('rtrw-popover-center-left') >= 0) {
            offsetX = 0;
            offsetY = -selfRect.height / 2;
        } else if (list.indexOf('rtrw-popover-center-center') >= 0) {
            offsetX = -selfRect.width / 2;
            offsetY = -selfRect.height / 2;
        } else if (list.indexOf('rtrw-popover-center-right') >= 0) {
            offsetX = -selfRect.width;
            offsetY = -selfRect.height / 2;
        }

        else if (list.indexOf('rtrw-popover-bottom-left') >= 0) {
            offsetX = 0;
            offsetY = -selfRect.height;
        } else if (list.indexOf('rtrw-popover-bottom-center') >= 0) {
            offsetX = -selfRect.width / 2;
            offsetY = -selfRect.height;
        } else if (list.indexOf('rtrw-popover-bottom-right') >= 0) {
            offsetX = -selfRect.width;
            offsetY = -selfRect.height;
        }

        return {
            top: top, left: left, offsetX: offsetX, offsetY: offsetY
        };
    },

    flipClassReplacements: {
        'top': {
            'rtrw-popover-top-left': 'rtrw-popover-bottom-left',
            'rtrw-popover-top-center': 'rtrw-popover-bottom-center',
            'rtrw-popover-anchor-bottom-center': 'rtrw-popover-anchor-top-center',
            'rtrw-popover-top-right': 'rtrw-popover-bottom-right',
        },
        'left': {
            'rtrw-popover-top-left': 'rtrw-popover-top-right',
            'rtrw-popover-center-left': 'rtrw-popover-center-right',
            'rtrw-popover-anchor-center-right': 'rtrw-popover-anchor-center-left',
            'rtrw-popover-bottom-left': 'rtrw-popover-bottom-right',
        },
        'right': {
            'rtrw-popover-top-right': 'rtrw-popover-top-left',
            'rtrw-popover-center-right': 'rtrw-popover-center-left',
            'rtrw-popover-anchor-center-left': 'rtrw-popover-anchor-center-right',
            'rtrw-popover-bottom-right': 'rtrw-popover-bottom-left',
        },
        'bottom': {
            'rtrw-popover-bottom-left': 'rtrw-popover-top-left',
            'rtrw-popover-bottom-center': 'rtrw-popover-top-center',
            'rtrw-popover-anchor-top-center': 'rtrw-popover-anchor-bottom-center',
            'rtrw-popover-bottom-right': 'rtrw-popover-top-right',
        },
        'top-and-left': {
            'rtrw-popover-top-left': 'rtrw-popover-bottom-right',
        },
        'top-and-right': {
            'rtrw-popover-top-right': 'rtrw-popover-bottom-left',
        },
        'bottom-and-left': {
            'rtrw-popover-bottom-left': 'rtrw-popover-top-right',
        },
        'bottom-and-right': {
            'rtrw-popover-bottom-right': 'rtrw-popover-top-left',
        },

    },

    flipMargin: 0,

    getPositionForFlippedPopover: function (inputArray, selector, boundingRect, selfRect) {
        const classList = [];
        for (var i = 0; i < inputArray.length; i++) {
            const item = inputArray[i];
            const replacements = window.rtrwPopoverHelper.flipClassReplacements[selector][item];
            if (replacements) {
                classList.push(replacements);
            }
            else {
                classList.push(item);
            }
        }

        return window.rtrwPopoverHelper.calculatePopoverPosition(classList, boundingRect, selfRect);
    },

    placePopover: function (popoverNode, classSelector) {

        if (popoverNode && popoverNode.parentNode) {
            const id = popoverNode.id.substr(8);
            const popoverContentNode = document.getElementById('popovercontent-' + id);
            if (popoverContentNode.classList.contains('rtrw-popover-open') == false) {
                return;
            }

            if (!popoverContentNode) {
                return;
            }

            if (classSelector) {
                if (popoverContentNode.classList.contains(classSelector) == false) {
                    return;
                }
            }
            const boundingRect = popoverNode.parentNode.getBoundingClientRect();

            if (popoverContentNode.classList.contains('rtrw-popover-relative-width')) {
                popoverContentNode.style['max-width'] = (boundingRect.width) + 'px';
            }

            const selfRect = popoverContentNode.getBoundingClientRect();
            const classList = popoverContentNode.classList;
            const classListArray = Array.from(popoverContentNode.classList);

            const postion = window.rtrwPopoverHelper.calculatePopoverPosition(classListArray, boundingRect, selfRect);
            let left = postion.left;
            let top = postion.top;
            let offsetX = postion.offsetX;
            let offsetY = postion.offsetY;

            if (classList.contains('rtrw-popover-overflow-flip-onopen') || classList.contains('rtrw-popover-overflow-flip-always')) {

                const appBarElements = document.getElementsByClassName("rtrw-appbar rtrw-appbar-fixed-top");
                let appBarOffset = 0;
                if (appBarElements.length > 0) {
                    appBarOffset = appBarElements[0].getBoundingClientRect().height;
                }

                const graceMargin = window.rtrwPopoverHelper.flipMargin;
                const deltaToLeft = left + offsetX;
                const deltaToRight = window.innerWidth - left - selfRect.width;
                const deltaTop = top - selfRect.height - appBarOffset;
                const spaceToTop = top - appBarOffset;
                const deltaBottom = window.innerHeight - top - selfRect.height;
                //console.log('self-width: ' + selfRect.width + ' | self-height: ' + selfRect.height);
                //console.log('left: ' + deltaToLeft + ' | rigth:' + deltaToRight + ' | top: ' + deltaTop + ' | bottom: ' + deltaBottom + ' | spaceToTop: ' + spaceToTop);

                let selector = popoverContentNode.rtrwPopoverFlipped;

                if (!selector) {
                    if (classList.contains('rtrw-popover-top-left')) {
                        if (deltaBottom < graceMargin && deltaToRight < graceMargin && spaceToTop >= selfRect.height && deltaToLeft >= selfRect.width) {
                            selector = 'top-and-left';
                        } else if (deltaBottom < graceMargin && spaceToTop >= selfRect.height) {
                            selector = 'top';
                        } else if (deltaToRight < graceMargin && deltaToLeft >= selfRect.width) {
                            selector = 'left';
                        }
                    } else if (classList.contains('rtrw-popover-top-center')) {
                        if (deltaBottom < graceMargin && spaceToTop >= selfRect.height) {
                            selector = 'top';
                        }
                    } else if (classList.contains('rtrw-popover-top-right')) {
                        if (deltaBottom < graceMargin && deltaToLeft < graceMargin && spaceToTop >= selfRect.height && deltaToRight >= selfRect.width) {
                            selector = 'top-and-right';
                        } else if (deltaBottom < graceMargin && spaceToTop >= selfRect.height) {
                            selector = 'top';
                        } else if (deltaToLeft < graceMargin && deltaToRight >= selfRect.width) {
                            selector = 'right';
                        }
                    }

                    else if (classList.contains('rtrw-popover-center-left')) {
                        if (deltaToRight < graceMargin && deltaToLeft >= selfRect.width) {
                            selector = 'left';
                        }
                    }
                    else if (classList.contains('rtrw-popover-center-right')) {
                        if (deltaToLeft < graceMargin && deltaToRight >= selfRect.width) {
                            selector = 'right';
                        }
                    }
                    else if (classList.contains('rtrw-popover-bottom-left')) {
                        if (deltaTop < graceMargin && deltaToRight < graceMargin && deltaBottom >= 0 && deltaToLeft >= selfRect.width) {
                            selector = 'bottom-and-left';
                        } else if (deltaTop < graceMargin && deltaBottom >= 0) {
                            selector = 'bottom';
                        } else if (deltaToRight < graceMargin && deltaToLeft >= selfRect.width) {
                            selector = 'left';
                        }
                    } else if (classList.contains('rtrw-popover-bottom-center')) {
                        if (deltaTop < graceMargin && deltaBottom >= 0) {
                            selector = 'bottom';
                        }
                    } else if (classList.contains('rtrw-popover-bottom-right')) {
                        if (deltaTop < graceMargin && deltaToLeft < graceMargin && deltaBottom >= 0 && deltaToRight >= selfRect.width) {
                            selector = 'bottom-and-right';
                        } else if (deltaTop < graceMargin && deltaBottom >= 0) {
                            selector = 'bottom';
                        } else if (deltaToLeft < graceMargin && deltaToRight >= selfRect.width) {
                            selector = 'right';
                        }
                    }
                }

                if (selector && selector != 'none') {
                    const newPosition = window.rtrwPopoverHelper.getPositionForFlippedPopover(classListArray, selector, boundingRect, selfRect);
                    left = newPosition.left;
                    top = newPosition.top;
                    offsetX = newPosition.offsetX;
                    offsetY = newPosition.offsetY;

                    popoverContentNode.setAttribute('data-rtrwpopover-flip', 'flipped');
                }
                else {
                    popoverContentNode.removeAttribute('data-rtrwpopover-flip');
                }

                if (classList.contains('rtrw-popover-overflow-flip-onopen')) {
                    if (!popoverContentNode.rtrwPopoverFlipped) {
                        popoverContentNode.rtrwPopoverFlipped = selector || 'none';
                    }
                }
            }

            if (popoverContentNode.classList.contains('rtrw-popover-fixed')) {
            }
            else if (window.getComputedStyle(popoverNode).position == 'fixed') {
                popoverContentNode.style['position'] = 'fixed';
            }
            else {
                offsetX += window.scrollX;
                offsetY += window.scrollY
            }

            popoverContentNode.style['left'] = (left + offsetX) + 'px';
            popoverContentNode.style['top'] = (top + offsetY) + 'px';

            if (window.getComputedStyle(popoverNode).getPropertyValue('z-index') != 'auto') {
                popoverContentNode.style['z-index'] = window.getComputedStyle(popoverNode).getPropertyValue('z-index');
                popoverContentNode.skipZIndex = true;
            }
        }
    },

    placePopoverByClassSelector: function (classSelector = null) {
        var items = window.rtrwPopover.getAllObservedContainers();

        for (let i = 0; i < items.length; i++) {
            const popoverNode = document.getElementById('popover-' + items[i]);
            window.rtrwPopoverHelper.placePopover(popoverNode, classSelector);
        }
    },

    placePopoverByNode: function (target) {
        const id = target.id.substr(15);
        const popoverNode = document.getElementById('popover-' + id);
        window.rtrwPopoverHelper.placePopover(popoverNode);
    }
}

class MudPopover {

    constructor() {
        this.map = {};
        this.contentObserver = null;
        this.mainContainerClass = null;
    }

    callback(id, mutationsList, observer) {
        for (const mutation of mutationsList) {
            if (mutation.type === 'attributes') {
                const target = mutation.target
                if (mutation.attributeName == 'class') {
                    if (target.classList.contains('rtrw-popover-overflow-flip-onopen') &&
                        target.classList.contains('rtrw-popover-open') == false) {
                        target.rtrwPopoverFliped = null;
                        target.removeAttribute('data-rtrwpopover-flip');
                    }

                    window.rtrwPopoverHelper.placePopoverByNode(target);
                }
                else if (mutation.attributeName == 'data-ticks') {
                    const tickAttribute = target.getAttribute('data-ticks');

                    const parent = target.parentElement;
                    const tickValues = [];
                    let max = -1;
                    for (let i = 0; i < parent.children.length; i++) {
                        const childNode = parent.children[i];
                        const tickValue = parseInt(childNode.getAttribute('data-ticks'));
                        if (tickValue == 0) {
                            continue;
                        }

                        if (tickValues.indexOf(tickValue) >= 0) {
                            continue;
                        }

                        tickValues.push(tickValue);

                        if (tickValue > max) {
                            max = tickValue;
                        }
                    }

                    if (tickValues.length == 0) {
                        continue;
                    }

                    const sortedTickValues = tickValues.sort((x, y) => x - y);

                    for (let i = 0; i < parent.children.length; i++) {
                        const childNode = parent.children[i];
                        const tickValue = parseInt(childNode.getAttribute('data-ticks'));
                        if (tickValue == 0) {
                            continue;
                        }

                        if (childNode.skipZIndex == true) {
                            continue;
                        }

                        childNode.style['z-index'] = 'calc(var(--rtrw-zindex-popover) + ' + (sortedTickValues.indexOf(tickValue) + 3).toString() + ')';
                    }
                }
            }
        }
    }

    initialize(containerClass, flipMargin) {
        const mainContent = document.getElementsByClassName(containerClass);
        if (mainContent.length == 0) {
            return;
        }

        if (flipMargin) {
            window.rtrwPopoverHelper.flipMargin = flipMargin;
        }

        this.mainContainerClass = containerClass;

        if (!mainContent[0].rtrwPopoverMark) {
            mainContent[0].rtrwPopoverMark = "rtrwded";
            if (this.contentObserver != null) {
                this.contentObserver.disconnect();
                this.contentObserver = null;
            }

            this.contentObserver = new ResizeObserver(entries => {
                window.rtrwPopoverHelper.placePopoverByClassSelector();
            });

            this.contentObserver.observe(mainContent[0]);
        }
    }

    connect(id) {
        this.initialize(this.mainContainerClass);

        const popoverNode = document.getElementById('popover-' + id);
        const popoverContentNode = document.getElementById('popovercontent-' + id);
        if (popoverNode && popoverNode.parentNode && popoverContentNode) {

            window.rtrwPopoverHelper.placePopover(popoverNode);

            const config = { attributeFilter: ['class', 'data-ticks'] };

            const observer = new MutationObserver(this.callback.bind(this, id));

            observer.observe(popoverContentNode, config);

            const resizeObserver = new ResizeObserver(entries => {
                for (let entry of entries) {
                    const target = entry.target;

                    for (var i = 0; i < target.childNodes.length; i++) {
                        const childNode = target.childNodes[i];
                        if (childNode.id && childNode.id.startsWith('popover-')) {
                            window.rtrwPopoverHelper.placePopover(childNode);
                        }
                    }
                }
            });

            resizeObserver.observe(popoverNode.parentNode);

            const contentNodeObserver = new ResizeObserver(entries => {
                for (let entry of entries) {
                    var target = entry.target;
                    window.rtrwPopoverHelper.placePopoverByNode(target);


                }
            });

            contentNodeObserver.observe(popoverContentNode);

            this.map[id] = {
                mutationObserver: observer,
                resizeObserver: resizeObserver,
                contentNodeObserver: contentNodeObserver
            };
        }
    }

    disconnect(id) {
        if (this.map[id]) {

            const item = this.map[id]
            item.mutationObserver.disconnect();
            item.resizeObserver.disconnect();
            item.contentNodeObserver.disconnect();

            delete this.map[id];
        }
    }

    dispose() {
        for (var i in this.map) {
            disconnect(i);
        }

        this.contentObserver.disconnect();
        this.contentObserver = null;
    }

    getAllObservedContainers() {
        const result = [];
        for (var i in this.map) {
            result.push(i);
        }

        return result;
    }
}

window.rtrwPopover = new MudPopover();

window.addEventListener('scroll', () => {
    window.rtrwPopoverHelper.placePopoverByClassSelector('rtrw-popover-fixed');
    window.rtrwPopoverHelper.placePopoverByClassSelector('rtrw-popover-overflow-flip-always');
});

window.addEventListener('resize', () => {
    window.rtrwPopoverHelper.placePopoverByClassSelector();
});

class RtrwScrollManager {
    //scrolls to an Id. Useful for navigation to fragments
    scrollToFragment(elementId, behavior) {
        let element = document.getElementById(elementId);

        if (element) {
            element.scrollIntoView({ behavior, block: 'center', inline: 'start' });
        }
    }

    //scrolls to year in RtrwDatePicker
    scrollToYear(elementId, offset) {
        let element = document.getElementById(elementId);

        if (element) {
            element.parentNode.scrollTop = element.offsetTop - element.parentNode.offsetTop - element.scrollHeight * 3;
        }
    }

    // sets the scroll position of the elements container, 
    // to the position of the element with the given element id
    scrollToListItem(elementId) {
        let element = document.getElementById(elementId);
        if (element) {
            let parent = element.parentElement;
            if (parent) {
                parent.scrollTop = element.offsetTop;
            }
        }
    }

    //scrolls to the selected element. Default is documentElement (i.e., html element)
    scrollTo(selector, left, top, behavior) {
        let element = document.querySelector(selector) || document.documentElement;
        element.scrollTo({ left, top, behavior });
    }

    scrollToBottom(selector, behavior) {
        let element = document.querySelector(selector);
        if (element)
            element.scrollTop = element.scrollHeight;
        else
            window.scrollTo(0, document.body.scrollHeight);
    }

    //locks the scroll of the selected element. Default is body
    lockScroll(selector, lockclass) {
        let element = document.querySelector(selector) || document.body;

        //if the body doesn't have a scroll bar, don't add the lock class
        let hasScrollBar = window.innerWidth > document.body.clientWidth;
        if (hasScrollBar) {
            element.classList.add(lockclass);
        }
    }

    //unlocks the scroll. Default is body
    unlockScroll(selector, lockclass) {
        let element = document.querySelector(selector) || document.body;
        element.classList.remove(lockclass);
    }
};
window.rtrwScrollManager = new RtrwScrollManager();