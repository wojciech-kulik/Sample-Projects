//
//  SmallImageCell.swift
//  GitHub Statistics
//
//  Created by Wojciech Kulik on 03/06/16.
//  Copyright © 2016 Wojciech Kulik. All rights reserved.
//

import Foundation
import UIKit

class SearchResultCell : UITableViewCell {
    
    override init(style: UITableViewCellStyle, reuseIdentifier: String?) {
        super.init(style: style, reuseIdentifier: reuseIdentifier)
        
        self.accessoryType = .DisclosureIndicator
        self.selectedBackgroundView = UIView()
        self.selectedBackgroundView!.backgroundColor =
            UIColor(red: 249.0 / 255.0, green: 206.0 / 255.0, blue: 48.0 / 255.0, alpha: 1)
    }
    
    required init?(coder aDecoder: NSCoder) {
        super.init(coder: aDecoder)
    }
    
    override func layoutSubviews() {
        super.layoutSubviews()
        self.imageView?.frame = CGRectMake(15, 2, 40, 40)
    }
}