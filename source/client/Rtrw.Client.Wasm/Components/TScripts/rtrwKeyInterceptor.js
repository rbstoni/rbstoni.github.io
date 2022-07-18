class RtrwKeyInterceptorFactory {

    connect(dotNetRef, elementId, options) {
        //console.log('[RtrwBlazor | RtrwKeyInterceptorFactory] connect ', { dotNetRef, element, options });
        if (!elementId)
            throw "elementId: expected element id!";
        var element = document.getElementById(elementId);
        if (!element)
            throw "no element found for id: " +elementId;
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
        this.logger('[RtrwBlazor | KeyInterceptor] Start observing DOM of element for changes to child with class ', { element, targetClass});
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
        this._observedChildren = this._observedChildren.filter(x=>x!==child);
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
        if (!option || option=== "none")
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

