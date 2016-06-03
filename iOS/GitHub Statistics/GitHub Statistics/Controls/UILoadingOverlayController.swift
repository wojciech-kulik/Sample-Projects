//
//  UILoadingOverlayController.swift
//  GitHub Statistics
//
//  Created by Wojciech Kulik on 03/06/16.
//  Copyright © 2016 Wojciech Kulik. All rights reserved.
//

import Foundation
import UIKit

public class UILoadingOverlayController : UIViewController {
    @IBOutlet public weak var spinner: UIActivityIndicatorView!
    @IBOutlet public weak var textLabel: UILabel!
    
    private var isShown = false
    
    public func show(parentView: UIView, frame: CGRect?) {
        if isShown {
            return
        }
        
        isShown = true
        self.view.frame = frame ?? parentView.bounds
        self.spinner.startAnimating()
        parentView.addSubview(self.view)
    }
    
    public func show(parentView: UIView) {
        show(parentView, frame: nil)
    }
    
    public func hide(parentView: UIView) {
        self.view.removeFromSuperview()
        self.spinner.stopAnimating()
        isShown = false
    }
}