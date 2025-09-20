import { Menu } from "antd";
import { Skeleton } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import type { Conversation } from "../../types/models/chat/conversation";
import { Link } from "react-router-dom";

interface NavigationMenuProps {
    conversations: Conversation[];
    handleAddPlanClick: (e: React.MouseEvent) => void;
    loading: boolean;
}

const NavigationMenu: React.FC<NavigationMenuProps> = ({ conversations, handleAddPlanClick, loading }) => {
    return (
        <Skeleton active loading={!!loading} style={{ padding: "16px" }}>
            <Menu style={{ paddingTop: "32px", paddingLeft: "8px" }}>
                <Menu.ItemGroup
                    key="g3"
                    title={
                        <div style={{ color: "gray", paddingLeft: 12, display: "flex", alignItems: "center", justifyContent: "space-between", gap: 8 }}>
                            <span>Conversations</span>
                            <PlusOutlined style={{ cursor: "pointer" }} onClick={handleAddPlanClick} />
                        </div>
                    }
                >
                    {conversations?.map((conversation: Conversation) => (
                        <Menu.Item key={conversation.id}>
                            <Link to={`/conversation/${conversation.id}`}>
                                <span style={{ textOverflow: "ellipsis", overflow: "hidden", whiteSpace: "nowrap", display: "block", width: "24ch" }}>
                                    {conversation.name}
                                </span>
                            </Link>
                        </Menu.Item>
                    ))}
                </Menu.ItemGroup>
            </Menu>
        </Skeleton>
    )
}
export default NavigationMenu;